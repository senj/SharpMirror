using Microsoft.Extensions.Logging;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Routes;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.Notifications;
using SmartMirror.SmartHome.Hue;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class IntentExecutor
    {
        private readonly ILogger<IntentExecutor> _logger;
        private readonly BringState _bringState;
        private readonly RouteState _routeState;
        private readonly SpotifyService _spotifyService;
        private readonly HueService _hueService;
        private readonly WeatherState _weatherState;

        public IntentExecutor(
            ILogger<IntentExecutor> logger,
            BringState bringState,
            RouteState routeState,
            SpotifyService spotifyService,
            HueService hueService,
            WeatherState weatherState)
        {
            _logger = logger;
            _bringState = bringState;
            _routeState = routeState;
            _spotifyService = spotifyService;
            _hueService = hueService;
            _weatherState = weatherState;
        }

        public Task<OneCallWeatherForecast> Handle(WeatherInformationRequest request)
        {
            return _weatherState.GetWeatherForecastAsync();
        }

        public Task Handle(WeatherDisplayType request)
        {
            _weatherState.SetShowDetails(request.DisplayForecast);
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
            _bringState.SetShowDetails(request.ShowDetails);
            return Task.CompletedTask;
        }

        public async Task Handle(AddListEntry request)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringState.AddItemAsync(entry, request.Details);
            }
        }

        public async Task Handle(RemoveListEntry request)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringState.RemoveItemAsync(entry);
            }
        }

        public Task<(RouteResponse route, GeosearchResponse source, GeosearchResponse destination)> Handle(GetDistanceRequest request)
        {
            return _routeState.FindRouteAsync(request.Source, request.Destination);
        }

        public Task Handle(RoutesDisplayType routesDisplayType)
        {
            _routeState.SetShowDetails(routesDisplayType.ShowDetails);
            return Task.CompletedTask;
        }
    }
}
