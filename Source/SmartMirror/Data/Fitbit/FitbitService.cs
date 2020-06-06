using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartMirror.Extensions;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Fitbit
{
    public class FitbitService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<FitbitService> _logger;
        private readonly HttpClient _httpClient;

        public FitbitService(
            IDistributedCache cache,
            ILogger<FitbitService> logger,
            HttpClient httpClient)
        {
            _cache = cache;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<FitbitUserResponse> GetUserInfoAsync(string token)
        {
            if (_cache.TryGetValue("fitbit_user", out FitbitUserResponse user))
            {
                _logger.LogInformation("[CACHE] Got fitbit user from cache");
                return user;
            }

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.fitbit.com/1/user/-/profile.json");
            request.Headers.Add("Authorization", $"Bearer {token}");
            
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting fitbit user data ({statusCode}): {response}", response.StatusCode, await response?.Content?.ReadAsStringAsync());
                return null;
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            FitbitUserResponse userResponse;
            try
            {
                userResponse = JsonSerializer.Deserialize<FitbitUserResponse>(stringResponse, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing fitbit user response");
                throw;
            }

            if (userResponse != null)
            {
                _cache.Set("fitbit_user", userResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
                });
            }

            return userResponse;
        }

        public async Task<FitbitSleepResponse> GetSleepOfAsync(string token, DateTime dateTime)
        {
            if (_cache.TryGetValue("fitbit_sleep", out FitbitSleepResponse sleep))
            {
                _logger.LogInformation("[CACHE] Got fitbit sleep from cache");
                return sleep;
            }

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://api.fitbit.com/1.2/user/-/sleep/date/{dateTime.ToString("yyyy-MM-dd")}.json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting fitbit sleep data ({statusCode})", response.StatusCode);
                return null;
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var sleepResponse = JsonSerializer.Deserialize<FitbitSleepResponse>(stringResponse, options);
            
            if (sleepResponse != null)
            {
                _cache.Set("fitbit_sleep", sleepResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
                });
            }

            return sleepResponse;
        }

        public async Task<FitbitDeviceResponse> GetDeviceAsync(string token)
        {
            if (_cache.TryGetValue("fitbit_device", out FitbitDeviceResponse device))
            {
                _logger.LogInformation("[CACHE] Got fitbit device from cache");
                return device;
            }

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.fitbit.com/1/user/-/devices.json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting fitbit device data ({statusCode})", response.StatusCode);
                return null;
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var deviceResponse = JsonSerializer.Deserialize<FitbitDeviceResponse>(stringResponse, options);

            if (deviceResponse != null)
            {
                _cache.Set("fitbit_device", deviceResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }

            return deviceResponse;
        }

        public async Task<ActiveMinutes> GetActiveMinutes(string type, string token, string date = "today", string time = "7d")
        {
            if (_cache.TryGetValue("fitbit_active_" + type, out ActiveMinutes active))
            {
                _logger.LogInformation($"[CACHE] Got fitbit_active_{type} from cache");
                return active;
            }

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://api.fitbit.com/1/user/-/activities/{type}/date/{date}/{time}.json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting fitbit active data ({statusCode})", response.StatusCode);
                return null;
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var activeMinutesResponse = JsonSerializer.Deserialize<ActiveMinutes>(stringResponse, options);
            
            if (activeMinutesResponse != null)
            {
                _cache.Set("fitbit_active_" + type, activeMinutesResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return activeMinutesResponse;
        }
    }
}
