using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Fuel
{
    public class FuelService
    {
        private const string FUEL_PRICES_CACHE_KEY = "fuelPrices";
        private const string FUEL_STATIONS_CACHE_KEY = "fuelStations";
        private readonly ILogger<FuelService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly FuelConfiguration _fuelConfiguration;
        private static FuelResponse fuelResponse = new FuelResponse();

        public FuelService(ILogger<FuelService> logger, HttpClient httpClient, IDistributedCache cache, IOptions<FuelConfiguration> fuelConfiguration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
            _fuelConfiguration = fuelConfiguration.Value;
        }

        public async Task<FuelResponse> GetFuelResponseAsync(int limit, bool useCache)
        {
            FuelStationResponse stationJsonResponse;
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            if (useCache && _cache.TryGetValue(FUEL_PRICES_CACHE_KEY, out FuelResponse cachedFuelResponse))
            {
                _logger.LogInformation("[CACHE] Got fuel prices from cache");
                return cachedFuelResponse;
            }

            if (_cache.TryGetValue(FUEL_STATIONS_CACHE_KEY, out FuelStationResponse stationCacheResponse))
            {
                _logger.LogInformation("[CACHE] Got gas stations from cache");
                stationJsonResponse = stationCacheResponse;
            }
            else
            {
                stationJsonResponse = await GetFuelStationResponseAsync();
                if (stationJsonResponse == null)
                {
                    return new FuelResponse();
                }

                _cache.Set(FUEL_STATIONS_CACHE_KEY, stationJsonResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4)
                });
            }

            string ids = string.Join(",", stationJsonResponse.Stations.Take(limit).Select(p => p.Id));

            string priceUrl = "https://creativecommons.tankerkoenig.de/json/prices.php" +
                $"?ids={ids}" +
                $"&apikey={_fuelConfiguration.ApiKey}";

            HttpResponseMessage priceResponse = await _httpClient.GetAsync(priceUrl);
            if (!priceResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting fuel prices: {statusCode}", priceResponse.StatusCode);
                return fuelResponse;
            }

            string stringPriceResponse = await priceResponse.Content.ReadAsStringAsync();
            stringPriceResponse = FixInvalidJsonStructure(stringPriceResponse);
            FuelPriceResponse priceJsonResponse = JsonSerializer.Deserialize<FuelPriceResponse>(stringPriceResponse, options);

            if (priceJsonResponse == null || priceJsonResponse.Ok == false)
            {
                _logger.LogError("Error getting fuel prices: {error}", stationJsonResponse?.Status);
                return fuelResponse;
            }

            List<FuelResult> fuelResults = new List<FuelResult>();
            foreach (Station station in stationJsonResponse.Stations)
            {
                if (station.IsOpen) 
                {
                    if (priceJsonResponse.Prices.TryGetValue(station.Id, out PriceStatus priceStatus))
                    {
                        fuelResults.Add(new FuelResult(DateTime.Now, station, priceStatus));
                    }
                }
            }

            _logger.LogInformation("{opened}/{closed} stations are opened.", fuelResults.Count, stationJsonResponse.Stations.Count());

            string currentTimestamp = DateTime.Now.ToString("dd.MM HH:mm");
            if (!fuelResponse.ContainsKey(currentTimestamp))
            {
                fuelResponse.Add(currentTimestamp, fuelResults);
            }

            if (fuelResponse.Keys.Count > 10)
            {
                fuelResponse.Remove(fuelResponse.Keys.First());
            }

            _cache.Set(FUEL_PRICES_CACHE_KEY, fuelResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            _logger.LogInformation("Got fuel response");
            return fuelResponse;
        }

        private async Task<FuelStationResponse> GetFuelStationResponseAsync()
        {
            string stationsUrl = "https://creativecommons.tankerkoenig.de/json/list.php" +
                       $"?lat={_fuelConfiguration.Lat}" +
                       $"&lng={_fuelConfiguration.Lon}" +
                       $"&rad={_fuelConfiguration.Radius}" +
                       $"&sort={_fuelConfiguration.Sort}" +
                       $"&type={_fuelConfiguration.Type}" +
                       $"&apikey={_fuelConfiguration.ApiKey}";

            HttpResponseMessage stationResponse = await _httpClient.GetAsync(stationsUrl);
            if (!stationResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting gas stations: {statusCode}", stationResponse.StatusCode);
                return null;
            }

            string stringStationResponse = await stationResponse.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            FuelStationResponse stationJsonResponse = JsonSerializer.Deserialize<FuelStationResponse>(stringStationResponse, options);

            if (stationJsonResponse == null || stationJsonResponse.Ok == false)
            {
                _logger.LogError("Error getting gas stations: {error}", stationJsonResponse?.Status);
                return null;
            }

            return stationJsonResponse;
        }

        private string FixInvalidJsonStructure(string stringPriceResponse)
        {
            if (stringPriceResponse.Contains("\"e5\":false"))
            {
                stringPriceResponse = stringPriceResponse.Replace("\"e5\":false", "\"e5\":0.0");
            }

            if (stringPriceResponse.Contains("\"e10\":false"))
            {
                stringPriceResponse = stringPriceResponse.Replace("\"e10\":false", "\"e10\":0.0");
            }

            if (stringPriceResponse.Contains("\"diesel\":false"))
            {
                stringPriceResponse = stringPriceResponse.Replace("\"diesel\":false", "\"diesel\":0.0");
            }

            return stringPriceResponse;
        }
    }
}
