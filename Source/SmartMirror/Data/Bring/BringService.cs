using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Bring
{
    public class BringService
    {
        private readonly ILogger<BringService> _logger;
        private readonly IOptions<BringConfiguration> _options;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private string _accessToken;
        private DateTime _expiresIn;
        private const string BRING_LIST_CACHE_KEY = "bring-list";

        public BringService(
            ILogger<BringService> logger,
            IOptions<BringConfiguration> options,
            HttpClient httpClient,
            IDistributedCache cache)
        {
            _logger = logger;
            _options = options;
            _httpClient = httpClient;
            _cache = cache;
            _expiresIn = DateTime.MinValue;
        }

        public async Task BringAuth()
        {
            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("email", _options.Value.Email),
                new KeyValuePair<string, string>("password", _options.Value.Password)
            };

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.getbring.com/rest/v2/bringauth");
            request.Headers.Add("X-BRING-API-KEY", "cof4Nc6D8saplXjE3h3HXqHH8m7VU2i1Gs0g85Sp");
            request.Headers.Add("X-BRING-CLIENT", "webApp");
            request.Headers.Add("X-BRING-USER-UUID", "1d803f70-ab3e-4420-8c97-f08e0efe7fbf");
            request.Headers.Add("X-BRING-VERSION", "303070050");
            request.Headers.Add("X-BRING-COUNTRY", "de");
            request.Content = new FormUrlEncodedContent(content);

            var response = await _httpClient.SendAsync(request);

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var bringAuthResponse = JsonSerializer.Deserialize<BringAuthResponse>(stringResponse, options);

            _accessToken = bringAuthResponse.access_token;
            _expiresIn = DateTime.UtcNow.AddSeconds(bringAuthResponse.expires_in);
        }

        public async Task<BringItemResponse> GetItemsAsync(bool loadFromCache = true)
        {
            if (loadFromCache && _cache.TryGetValue(BRING_LIST_CACHE_KEY, out BringItemResponse bringItemResponse))
            {
                _logger.LogInformation("[CACHE] Got bring list from cache");
                return bringItemResponse;
            }

            if (DateTime.UtcNow >= _expiresIn)
            {
                await BringAuth();
            }

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://api.getbring.com/rest/v2/bringlists/{_options.Value.ListId}");
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            request.Headers.Add("X-BRING-API-KEY", "cof4Nc6D8saplXjE3h3HXqHH8m7VU2i1Gs0g85Sp");
            request.Headers.Add("X-BRING-CLIENT", "webApp");
            request.Headers.Add("X-BRING-USER-UUID", "1d803f70-ab3e-4420-8c97-f08e0efe7fbf");
            request.Headers.Add("X-BRING-VERSION", "303070050");
            request.Headers.Add("X-BRING-COUNTRY", "de");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting bring items: {statusCode}", response.StatusCode);
                return new BringItemResponse();
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            bringItemResponse = JsonSerializer.Deserialize<BringItemResponse>(stringResponse, options);

            if (bringItemResponse != null)
            {
                _cache.Set(BRING_LIST_CACHE_KEY, bringItemResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }

            _logger.LogInformation("Got items from bring api");
            return bringItemResponse;
        }

        public async Task AddItemAsync(string itemName, string details)
        {
            if (DateTime.UtcNow >= _expiresIn)
            {
                await BringAuth();
            }

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"https://api.getbring.com/rest/v2/bringlists/{_options.Value.ListId}");
            request.Content = new StringContent($"purchase={itemName}&recently=&specification={details}&remove=&sender=null", Encoding.UTF8, "application/x-www-form-urlencoded");

            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            request.Headers.Add("X-BRING-API-KEY", "cof4Nc6D8saplXjE3h3HXqHH8m7VU2i1Gs0g85Sp");
            request.Headers.Add("X-BRING-CLIENT", "webApp");
            request.Headers.Add("X-BRING-USER-UUID", "1d803f70-ab3e-4420-8c97-f08e0efe7fbf");
            request.Headers.Add("X-BRING-VERSION", "303070050");
            request.Headers.Add("X-BRING-COUNTRY", "de");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error adding bring item: {statusCode}", response.StatusCode);
            }

            _logger.LogInformation("Added item to bring shopping list.");
        }

        public async Task RemoveItemAsync(string itemName)
        {
            if (DateTime.UtcNow >= _expiresIn)
            {
                await BringAuth();
            }

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"https://api.getbring.com/rest/v2/bringlists/{_options.Value.ListId}");
            request.Content = new StringContent($"purchase=&recently=&specification=&remove={itemName}&sender=null", Encoding.UTF8, "application/x-www-form-urlencoded");

            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            request.Headers.Add("X-BRING-API-KEY", "cof4Nc6D8saplXjE3h3HXqHH8m7VU2i1Gs0g85Sp");
            request.Headers.Add("X-BRING-CLIENT", "webApp");
            request.Headers.Add("X-BRING-USER-UUID", "1d803f70-ab3e-4420-8c97-f08e0efe7fbf");
            request.Headers.Add("X-BRING-VERSION", "303070050");
            request.Headers.Add("X-BRING-COUNTRY", "de");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error adding bring item: {statusCode}", response.StatusCode);
            }

            _logger.LogInformation("Removed item to bring shopping list.");
        }
    }
}
