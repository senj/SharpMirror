using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Routes
{
    public class RouteService
    {
        private readonly ILogger<RouteService> _logger;
        private readonly HttpClient _httpClient;
        private readonly RouteConfiguration _configuration;

        public RouteService(
            ILogger<RouteService> logger,
            HttpClient httpClient,
            IOptions<RouteConfiguration> configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration.Value;
        }
     
        public async Task<GeosearchResponse> SearchAsync(string query)
        {
            string requestUri = "https://atlas.microsoft.com/search/address/json" +
               $"?subscription-key={_configuration.ApiKey}" +
               $"&api-version={_configuration.ApiVersion}" +
               $"&query={query}";
                
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting geo result: {statusCode}", response.StatusCode);
                return new GeosearchResponse();
            }

            var stringResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var geoJsonResponse = JsonSerializer.Deserialize<GeosearchResponse>(stringResponse, options);

            if (geoJsonResponse == null)
            {
                _logger.LogError("Error getting geo results, response is null");
                return new GeosearchResponse();
            }

            return geoJsonResponse;
        }

        public async Task<RouteResponse> GetRouteAsync(RouteRequest routeRequest)
        {
            string requestUri = "https://atlas.microsoft.com/route/directions/json" +
                $"?subscription-key={_configuration.ApiKey}" +
                $"&api-version={_configuration.ApiVersion}" +
                $"&query={routeRequest.Departure.Latitude},{routeRequest.Departure.Longitude}:{routeRequest.Destination.Latitude},{routeRequest.Destination.Longitude}" +
                $"&computeTravelTimeFor=all" +
                $"&traffic=true";
            
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting route result: {statusCode}", response.StatusCode);
                return new RouteResponse();
            }

            var stringResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var routeJsonResponse = JsonSerializer.Deserialize<RouteResponse>(stringResponse, options);

            if (routeJsonResponse == null)
            {
                _logger.LogError("Error getting route results, response is null");
                return new RouteResponse();
            }

            return routeJsonResponse;
        }
    }
}
