using Microsoft.Extensions.Logging;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Calendar;
using SmartMirror.Data.Clock;
using SmartMirror.Data.Fitbit;
using SmartMirror.Data.Fuel;
using SmartMirror.Data.Routes;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.VVS;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.Intents;
using SmartMirror.SmartHome.Hue;
using System;
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
        private readonly ClockState _clockState;
        private readonly VvsState _vvsState;
        private readonly FitbitState _fitbitState;

        public IntentExecutor(
            ILogger<IntentExecutor> logger,
            BringState bringState,
            RouteState routeState,
            SpotifyState spotifyState,
            HueState hueState,
            WeatherState weatherState,
            CalendarState calendarState,
            FuelState fuelState,
            ClockState clockState,
            VvsState vvsState,
            FitbitState fitbitState)
        {
            _logger = logger;
            _bringState = bringState;
            _routeState = routeState;
            _spotifyState = spotifyState;
            _hueState = hueState;
            _weatherState = weatherState;
            _calendarState = calendarState;
            _fuelState = fuelState;
            _clockState = clockState;
            _vvsState = vvsState;
            _fitbitState = fitbitState;
        }

        internal Task<OneCallWeatherForecast> Handle(WeatherQueryWeather request)
        {
            return _weatherState.GetWeatherForecastAsync();
        }

        internal Task Handle(WeatherDisplayForecast request)
        {
            _weatherState.SetShowDetails(request.DisplayForecast);
            return Task.CompletedTask;
        }

        internal async Task Handle(HueTurnOn notification)
        {
            LightState lightState = new LightState
            {
                on = true,
                sat = 0,
                bri = 127
            };

            await _hueState.SetLightStateAsync(notification.LightId, lightState);
        }

        internal async Task Handle(HueTurnOff notification)
        {
            LightState lightState = new LightState
            {
                on = false
            };

            await _hueState.SetLightStateAsync(notification.LightId, lightState);
        }

        internal Task Handle(HueDisplayDetails hueDisplayDetails)
        {
            _hueState.SetShowDetails(hueDisplayDetails.ShowDetails);
            return Task.CompletedTask;
        }

        internal Task Handle(HueShow hueShow)
        {
            _hueState.SetEnabled(hueShow.DisplayHue); 
            return Task.CompletedTask;
        }

        internal Task Handle(SpotifyNextSong notification)
        {
            return _spotifyState.PlayNextSongAsync();
        }

        internal Task Handle(SpotifyShow spotifyShow)
        {
            _spotifyState.SetEnabled(spotifyShow.DisplaySpotify);
            return Task.CompletedTask;
        }

        internal Task Handle(BringDisplayDetails request)
        {
            _bringState.SetShowDetails(request.ShowDetails);
            return Task.CompletedTask;
        }

        internal async Task Handle(BringAddToDo request)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringState.AddItemAsync(entry, request.Details);
            }
        }

        internal async Task Handle(BringDeleteToDo request)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringState.RemoveItemAsync(entry);
            }
        }

        internal Task Handle(BringShow bringShow)
        {
            _bringState.SetEnabled(bringShow.DisplayBring);
            return Task.CompletedTask;
        }

        internal Task<(RouteResponse route, GeosearchResponse source, GeosearchResponse destination)> Handle(RoutesGetRoute request)
        {
            return _routeState.FindRouteAsync(request.Source, request.Destination);
        }

        internal Task Handle(RoutesDisplayDetails routesDisplayType)
        {
            _routeState.SetShowDetails(routesDisplayType.ShowDetails);
            return Task.CompletedTask;
        }
        
        internal Task Handle(RoutesShow routesShow)
        {
            _routeState.SetEnabled(routesShow.DisplayRoutes);
            return Task.CompletedTask;
        }

        internal Task Handle(WeatherShow weatherShow)
        {
            _weatherState.SetEnabled(weatherShow.DisplayWeather);
            return Task.CompletedTask;
        }

        internal Task Handle(CalendarDisplayDays setCalendarDays)
        {
            return _calendarState.GetEventsAsync(setCalendarDays.NumberOfDays);
        }

        internal Task Handle(CalendarShow calendarShow)
        {
            _calendarState.SetEnabled(calendarShow.DisplayCalendar);
            return Task.CompletedTask;
        }

        internal Task Handle(FuelRefresh fuelRefresh)
        {
            return _fuelState.GetFuelResponseAsync(useCache: false);
        }

        internal Task Handle(FuelDisplayDetails fuelDisplayDetails)
        {
            _fuelState.SetShowDetails(fuelDisplayDetails.ShowDetails);
            return Task.CompletedTask;
        }

        internal Task Handle(FuelShow fuelShow)
        {
            _fuelState.SetEnabled(fuelShow.DisplayFuel);
            return Task.CompletedTask;
        }

        internal Task Handle(ClockTimer clockTimer)
        {
            _clockState.StopTimer();
            _clockState.SetTimer(clockTimer.Name, clockTimer.DurationSeconds);
            return Task.CompletedTask;
        }

        internal Task Handle(ClockShow clockShow)
        {
            _clockState.SetEnabled(clockShow.DisplayClock);
            return Task.CompletedTask;
        }

        internal Task Handle(StopClockTimer stopClockTimer)
        {
            _clockState.StopTimer();
            return Task.CompletedTask;
        }

        internal Task Handle(VvsShow vvsShow)
        {
            _vvsState.SetEnabled(vvsShow.DisplayVvs);
            return Task.CompletedTask;
        }

        internal Task Handle(FitbitShow fitbitShow)
        {
            _fitbitState.SetEnabled(fitbitShow.DisplayFitbit);
            return Task.CompletedTask;
        }

        internal Task Handle(MirrorShow mirrorShow)
        {
            _bringState.SetEnabled(mirrorShow.ShowWidgets);
            _calendarState.SetEnabled(mirrorShow.ShowWidgets);
            _fuelState.SetEnabled(mirrorShow.ShowWidgets);
            _weatherState.SetEnabled(mirrorShow.ShowWidgets);
            _spotifyState.SetEnabled(mirrorShow.ShowWidgets);
            _routeState.SetEnabled(mirrorShow.ShowWidgets);
            _clockState.SetEnabled(mirrorShow.ShowWidgets);
            _vvsState.SetEnabled(mirrorShow.ShowWidgets);
            _fitbitState.SetEnabled(mirrorShow.ShowWidgets);

            return Task.CompletedTask;
        }
    }
}
