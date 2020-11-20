using Microsoft.Extensions.Logging;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Calendar;
using SmartMirror.Data.Fuel;
using SmartMirror.Data.Routes;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.Intents;
using SmartMirror.SmartHome.Hue;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class IntentExecutor
    {
        private readonly ILogger<IntentExecutor> _logger;
        private readonly BringState _bringState;
        private readonly RouteState _routeState;
        private readonly SpotifyState _spotifyState;
        private readonly HueState _hueState;
        private readonly WeatherState _weatherState;
        private readonly CalendarState _calendarState;
        private readonly FuelState _fuelState;

        public IntentExecutor(
            ILogger<IntentExecutor> logger,
            BringState bringState,
            RouteState routeState,
            SpotifyState spotifyState,
            HueState hueState,
            WeatherState weatherState,
            CalendarState calendarState,
            FuelState fuelState)
        {
            _logger = logger;
            _bringState = bringState;
            _routeState = routeState;
            _spotifyState = spotifyState;
            _hueState = hueState;
            _weatherState = weatherState;
            _calendarState = calendarState;
            _fuelState = fuelState;
        }

        public Task<OneCallWeatherForecast> Handle(WeatherQueryWeather request)
        {
            return _weatherState.GetWeatherForecastAsync();
        }

        public Task Handle(WeatherDisplayForecast request)
        {
            _weatherState.SetShowDetails(request.DisplayForecast);
            return Task.CompletedTask;
        }

        public async Task Handle(HueTurnOn notification)
        {
            LightState lightState = new LightState
            {
                on = true,
                sat = 0,
                bri = 127
            };

            await _hueState.SetLightStateAsync(notification.LightId, lightState);
        }

        public async Task Handle(HueTurnOff notification)
        {
            LightState lightState = new LightState
            {
                on = false
            };

            await _hueState.SetLightStateAsync(notification.LightId, lightState);
        }

        public Task Handle(HueDisplayDetails hueDisplayDetails)
        {
            _hueState.SetShowDetails(hueDisplayDetails.ShowDetails);
            return Task.CompletedTask;
        }

        public Task Handle(SpotifyNextSong notification)
        {
            return _spotifyState.PlayNextSongAsync();
        }

        public Task Handle(BringDisplayDetails request)
        {
            _bringState.SetShowDetails(request.ShowDetails);
            return Task.CompletedTask;
        }

        public async Task Handle(BringAddToDo request)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringState.AddItemAsync(entry, request.Details);
            }
        }

        public async Task Handle(BringDeleteToDo request)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringState.RemoveItemAsync(entry);
            }
        }

        public Task<(RouteResponse route, GeosearchResponse source, GeosearchResponse destination)> Handle(RoutesGetRoute request)
        {
            return _routeState.FindRouteAsync(request.Source, request.Destination);
        }

        public Task Handle(RoutesDisplayDetails routesDisplayType)
        {
            _routeState.SetShowDetails(routesDisplayType.ShowDetails);
            return Task.CompletedTask;
        }

        public Task Handle(CalendarDisplayDays setCalendarDays)
        {
            return _calendarState.GetEventsAsync(setCalendarDays.NumberOfDays);
        }

        public Task Handle(FuelRefresh fuelRefresh)
        {
            return _fuelState.GetFuelResponseAsync(useCache: false);
        }

        public Task Handle(FuelDisplayDetails fuelDisplayDetails)
        {
            _fuelState.SetShowDetails(fuelDisplayDetails.ShowDetails);
            return Task.CompletedTask;
        }
    }
}
