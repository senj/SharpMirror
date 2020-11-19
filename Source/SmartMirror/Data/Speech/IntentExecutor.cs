using Microsoft.Extensions.Logging;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Routes;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.Notifications;
using SmartMirror.SmartHome.Hue;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class IntentExecutor
    {
        private readonly ILogger<IntentExecutor> _logger;
        private readonly BringService _bringService;
        private readonly RouteService _routeService;
        private readonly SpotifyService _spotifyService;
        private readonly HueService _hueService;
        private readonly WeatherForecastService _weatherForecastService;

        public IntentExecutor(
            ILogger<IntentExecutor> logger,
            BringService bringService,
            RouteService routeService,
            SpotifyService spotifyService,
            HueService hueService,
            WeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _bringService = bringService;
            _routeService = routeService;
            _spotifyService = spotifyService;
            _hueService = hueService;
            _weatherForecastService = weatherForecastService;
        }

        public Task<OneCallWeatherForecast> Handle(WeatherInformationRequest request)
        {
            return _weatherForecastService.GetOneCallForecastAsync(request.DisplayForecast);
        }

        public Task Handle(WeatherDisplayType request)
        {
            _weatherForecastService.WeatherDisplayType(request.DisplayForecast);
            return Task.CompletedTask;
        }

        public async Task Handle(TurnOn notification)
        {
            LightState lightState = new LightState
            {
                on = true,
                sat = 0,
                bri = 127
            };

            await _hueService.SetLightStateAsync(notification.LightId, lightState);
        }

        public async Task Handle(TurnOff notification)
        {
            LightState lightState = new LightState
            {
                on = false
            };

            await _hueService.SetLightStateAsync(notification.LightId, lightState);
        }
        public Task Handle(NextSongRequested notification)
        {
            _spotifyService.PlayNextSong();
            return Task.CompletedTask;
        }

        public Task Handle(ShoppingListDisplayType request)
        {
            _bringService.SetShoppingListDisplayType(request.ShowDetails);
            return Task.CompletedTask;
        }

        public async Task Handle(AddListEntry request)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringService.AddItemAsync(entry, request.Details);
            }
        }

        public async Task<RouteResponse> Handle(GetDistanceRequest request)
        {
            GeosearchResponse source = await _routeService.SearchAsync(request.Source);
            GeosearchResponse destination = await _routeService.SearchAsync(request.Destination);

            if (source != null && destination != null)
            {
                RouteResponse routeResponse = await _routeService.GetRouteAsync(new RouteRequest
                {
                    Departure = new Geopoint
                    {
                        Latitude = source.results.FirstOrDefault().position.lat,
                        Longitude = source.results.FirstOrDefault().position.lon
                    },
                    Destination = new Geopoint
                    {
                        Latitude = destination.results.FirstOrDefault().position.lat,
                        Longitude = destination.results.FirstOrDefault().position.lon
                    }
                });

                _routeService.DisplayRoute(source, destination, routeResponse);
            }

            return new RouteResponse();
        }
    }
}
