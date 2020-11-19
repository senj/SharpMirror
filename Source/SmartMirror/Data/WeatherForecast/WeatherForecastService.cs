using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Extensions;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.WeatherForecast
{
    public class WeatherForecastService
    {
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly IDistributedCache _cache;
        private readonly HttpClient _httpClient;
        private readonly WeatherConfiguration _configuration;

        public WeatherForecastService(
            ILogger<WeatherForecastService> logger,
            IOptions<WeatherConfiguration> configuration,
            IDistributedCache cache,
            HttpClient httpClient)
        {
            _logger = logger;
            _cache = cache;
            _httpClient = httpClient;
            _configuration = configuration.Value;
        }

        public async Task<OneCallWeatherForecast> GetOneCallForecastAsync()
        {
            if (_cache.TryGetValue("weather", out OneCallWeatherForecast currentWeather))
            {
                _logger.LogInformation("[CACHE] Got weather forecast from cache");
                return currentWeather;
            }

            // icons: https://openweathermap.org/weather-conditions
            var apiCall = $"https://api.openweathermap.org/data/2.5/onecall" +
                $"?lat={_configuration.Lat}" +
                $"&lon={_configuration.Lon}" +
                $"&appid={_configuration.ApiKey}" +
                $"&units={_configuration.Unit}" +
                $"&lang={_configuration.Language}";

            var response = await _httpClient.GetAsync(apiCall);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting weather forecast: {statusCode}", response.StatusCode);
                return new OneCallWeatherForecast();
            }

            var responseObj = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var oneCallWeatherForecast = JsonSerializer.Deserialize<OneCallWeatherForecast>(responseObj, options);

            if (oneCallWeatherForecast?.Current == null)
            {
                _logger.LogError("Error getting weather forecast, response is null");
                return new OneCallWeatherForecast();
            }

            _cache.Set("weather", oneCallWeatherForecast, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            });

            _logger.LogInformation("Got weather forecast");
            return oneCallWeatherForecast;
        }
    }
}