using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;
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
        private readonly NumberFormatInfo _numberFormatInfo;

        public RouteService(
            ILogger<RouteService> logger,
            HttpClient httpClient,
            IOptions<RouteConfiguration> configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration.Value;

            _numberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
        }

        public async Task<(RouteResponse route, GeosearchResponse source, GeosearchResponse destination)> FindRouteAsync(string source, string destination)
        {
            GeosearchResponse geoSource = await SearchAsync(source);
            GeosearchResponse geoDestination = await SearchAsync(destination);

            if (source != null && destination != null)
            {
                return (await GetRouteAsync(new RouteRequest
                {
                    Departure = new Geopoint
                    {
                        Latitude = geoSource.results.FirstOrDefault().position.lat,
                        Longitude = geoSource.results.FirstOrDefault().position.lon
                    },
                    Destination = new Geopoint
                    {
                        Latitude = geoDestination.results.FirstOrDefault().position.lat,
                        Longitude = geoDestination.results.FirstOrDefault().position.lon
                    }
                }), geoSource, geoDestination);
            }

            return (new RouteResponse(), new GeosearchResponse(), new GeosearchResponse());
        }

        private async Task<GeosearchResponse> SearchAsync(string query)
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

        private async Task<RouteResponse> GetRouteAsync(RouteRequest routeRequest)
        {
            string requestUri = "https://atlas.microsoft.com/route/directions/json" +
                $"?subscription-key={_configuration.ApiKey}" +
                $"&api-version={_configuration.ApiVersion}" +
                $"&query={routeRequest.Departure.Latitude.ToString(_numberFormatInfo)},{routeRequest.Departure.Longitude.ToString(_numberFormatInfo)}" +
                $":{routeRequest.Destination.Latitude.ToString(_numberFormatInfo)},{routeRequest.Destination.Longitude.ToString(_numberFormatInfo)}" +
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
