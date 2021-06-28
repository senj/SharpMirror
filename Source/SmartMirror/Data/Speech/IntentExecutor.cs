using Microsoft.Extensions.Logging;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Calendar;
using SmartMirror.Data.Clock;
using SmartMirror.Data.Fitbit;
using SmartMirror.Data.Fuel;
using SmartMirror.Data.GoogleFit;
using SmartMirror.Data.News;
using SmartMirror.Data.Routes;
using SmartMirror.Data.Soccer;
using SmartMirror.Data.Spotify;
using SmartMirror.Data.VVS;
using SmartMirror.Data.WeatherForecast;
using SmartMirror.Intents;
using SmartMirror.Intents.Show;
using SmartMirror.SmartHome.Hue;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class IntentExecutor
    {
        private readonly Dictionary<Type, Displayable> _displayableDictionary;

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
        //private readonly FitbitState _fitbitState;
        private readonly GoogleFitState _googleFitState;
        private readonly SoccerState _bundesligaState;
        private readonly NewsState _newsState;

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
            //FitbitState fitbitState,
            GoogleFitState googleFitState,
            SoccerState bundesligaState,
            NewsState newsState)
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
            //_fitbitState = fitbitState;
            _googleFitState = googleFitState;
            _bundesligaState = bundesligaState;
            _newsState = newsState;

            _displayableDictionary = new Dictionary<Type, Displayable>
            {
                { typeof(BringState), bringState },
                { typeof(RouteState), routeState },
                { typeof(SpotifyState), spotifyState },
                { typeof(HueState), hueState },
                { typeof(WeatherState), weatherState },
                { typeof(CalendarState), calendarState },
                { typeof(FuelState), fuelState },
                { typeof(ClockState), clockState },
                { typeof(VvsState), vvsState },
                //{ typeof(FitbitState), fitbitState },
                { typeof(GoogleFitState), googleFitState },
                { typeof(SoccerState), bundesligaState },
                { typeof(NewsState), newsState },
            };
        }

        internal Task<OneCallWeatherForecast> Handle(WeatherQueryWeather request)
        {
            return _weatherState.GetWeatherForecastAsync();
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

        internal Task Handle(SpotifyNextSong notification)
        {
            return _spotifyState.PlayNextSongAsync();
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

        internal Task<(RouteResponse route, GeosearchResponse source, GeosearchResponse destination)> Handle(RoutesGetRoute request)
        {
            return _routeState.FindRouteAsync(request.Source, request.Destination);
        }

        internal Task Handle(CalendarDisplayDays setCalendarDays)
        {
            _calendarState.SetEnabled(true);
            return _calendarState.GetEventsAsync(setCalendarDays.NumberOfDays);
        }

        internal Task Handle(FuelRefresh fuelRefresh)
        {
            return _fuelState.GetFuelResponseAsync(useCache: false);
        }

        internal Task Handle(ClockTimer clockTimer)
        {
            _clockState.StopTimer();
            _clockState.SetTimer(clockTimer.Name, clockTimer.DurationSeconds);
            return Task.CompletedTask;
        }

        internal Task Handle(ClockShow clockShow)
        {
            _clockState.SetEnabled(clockShow.Display);
            return Task.CompletedTask;
        }

        internal Task Handle(StopClockTimer stopClockTimer)
        {
            _clockState.StopTimer();
            return Task.CompletedTask;
        }

        internal Task Handle<T>(BaseDisplayDetails displayDetails) where T : Displayable
        {
            Displayable state = _displayableDictionary[typeof(T)];
            state.SetEnabled(true);
            state.SetShowDetails(displayDetails.Details);
            return Task.CompletedTask;
        }

        internal Task Handle<T>(BaseDisplayWidget displayWidget) where T : Displayable
        {
            Displayable state = _displayableDictionary[typeof(T)];
            state.SetEnabled(displayWidget.Display);

            return Task.CompletedTask;
        }

        internal Task Handle(MirrorShow mirrorShow)
        {
            foreach (KeyValuePair<Type, Displayable> entry in _displayableDictionary)
            {
                entry.Value.SetEnabled(mirrorShow.Display);
            }

            return Task.CompletedTask;
        }
    }
}
