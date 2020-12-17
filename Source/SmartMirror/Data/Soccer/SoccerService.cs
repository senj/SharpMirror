using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartMirror.Extensions;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Soccer
{
    public class SoccerService
    {
        private const string SOCCER_CACHE_KEY = "buliga";
        private readonly ILogger<SoccerService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public SoccerService(ILogger<SoccerService> logger, HttpClient httpClient, IDistributedCache cache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<BundesligaModel> GetCurrentPlayDayAsync()
        {
            if (_cache.TryGetValue(SOCCER_CACHE_KEY, out BundesligaModel cachedModel))
            {
                _logger.LogInformation("[CACHE] Got soccer results from cache");
                return cachedModel;
            }

            string baseUrl = "https://www.openligadb.de/api/getmatchdata/bl1";
            var response = await _httpClient.GetAsync(baseUrl);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting match data: {statusCode}", response.StatusCode);
                return new BundesligaModel();
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var playDayResponse = JsonSerializer.Deserialize<BundesligaModel>(stringResponse, options);

            if (playDayResponse == null)
            {
                _logger.LogError("Error getting match data, playDayResponse is null.");
                return new BundesligaModel();
            }

            _cache.Set(SOCCER_CACHE_KEY, playDayResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });

            _logger.LogInformation("Got playday results");
            return playDayResponse;
        }
    }
}
