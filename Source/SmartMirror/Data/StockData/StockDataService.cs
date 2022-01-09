using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartMirror.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.StockData
{
    public class StockDataService
    {
        private readonly ILogger<StockDataService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly string _apiKey;

        public StockDataService(ILogger<StockDataService> logger, HttpClient httpClient, IDistributedCache cache, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
            _apiKey = configuration.GetValue<string>("StockConfiguration:ApiKey");
        }

        public async Task<StockViewModel> GetStockDataAsync(string symbol)
        {
            if (_cache.TryGetValue(symbol, out StockViewModel cachedModel))
            {
                _logger.LogInformation("[CACHE] Got stock information for {symbol} from cache", symbol);
                return cachedModel;
            }

            string baseUrl = "https://www.alphavantage.co/query?" +
                "function=TIME_SERIES_INTRADAY" +
                $"&symbol={symbol}" +
                $"&interval=5min" +
                $"&apikey={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(baseUrl);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Unable to get stock information for {symbol}: {statusCode}", symbol, response.StatusCode);
                return new StockViewModel();
            }

            string stringResponse = await response.Content.ReadAsStringAsync();
            stringResponse = stringResponse.Replace(" ", "");
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
            StockDataModel stockResponse = JsonSerializer.Deserialize<StockDataModel>(stringResponse, options);

            if (stockResponse == null || !stockResponse.TimeSeries.HasValue)
            {
                _logger.LogError("Stock response is invalid");
                return new StockViewModel();
            }

            // Trim down for only today
            IEnumerable<JsonProperty> stockData = stockResponse.TimeSeries.Value.EnumerateObject()
                .Where(p => DateTime.ParseExact(p.Name, "yyyy-MM-ddHH:mm:ss", null).ToString("dd.MM.") == DateTime.Now.ToString("dd.MM."))
                .Select(p => p);

            StockViewModel stockViewModel = new() { StockData = stockData };

            _cache.Set(symbol, stockViewModel, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(25)
            });

            _logger.LogInformation("Got stock information for {symbol}", symbol);
            return stockViewModel;
        }
    }
}
