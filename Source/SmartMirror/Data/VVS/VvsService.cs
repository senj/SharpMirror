using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartMirror.Extensions;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.VVS
{
    public class VvsService
    {
        private readonly ILogger<VvsService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public VvsService(ILogger<VvsService> logger, HttpClient httpClient, IDistributedCache cache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<VvsResponse> GetVvsResponseAsync(int limit, int[] filter)
        {
            if (_cache.TryGetValue("vvs", out VvsResponse vvsResponse))
            {
                _logger.LogInformation("[CACHE] Got VVS response from cache.");
                return vvsResponse;
            }

            string stationId = "de:08116:4257";
            string baseUrl = "https://www3.vvs.de/mngvvs/XML_DM_REQUEST" +
                $"?limit={30}" +
                "&mode=direct" +
                $"&name_dm={stationId}" +
                "&outputFormat=rapidJSON" +
                "&type_dm=any" +
                "&useRealtime=1";

            HttpResponseMessage response = await _httpClient.GetAsync(baseUrl);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting VVS results: {statusCode}", response.StatusCode);
                return new VvsResponse();
            }

            string stringResponse = await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            VvsResponse vvsJsonResponse = JsonSerializer.Deserialize<VvsResponse>(stringResponse, options);

            if (vvsJsonResponse == null)
            {
                _logger.LogError("Error getting VVS results, response is null");
                return new VvsResponse();
            }

            vvsJsonResponse.StopEvents = vvsJsonResponse.StopEvents
                .Where(p => filter.Contains(p.Transportation.product.id))
                .Take(limit)
                .ToArray();

            _cache.Set("vvs", vvsJsonResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            _logger.LogInformation("Got VVS result");
            return vvsJsonResponse;
        }
    }
}
