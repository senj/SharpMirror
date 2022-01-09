using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Calendar;
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
using SmartMirror.SmartHome.Hue;
using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class SpeechRecognitionService
    {
        private readonly ILogger<SpeechRecognitionService> _logger;
        private readonly IntentExecutor _intentExecutor;
        private readonly SpeechRecognitionConfiguration _config;
        private readonly LUISRuntimeClient _luisClient;

        public SpeechRecognitionService(
            ILogger<SpeechRecognitionService> logger,
            IOptions<SpeechRecognitionConfiguration> speechRecognitionConfiguration,
            IntentExecutor intentExecutor)
        {
            _logger = logger;
            _intentExecutor = intentExecutor;
            _config = speechRecognitionConfiguration.Value;

            ApiKeyServiceClientCredentials credentials = new(_config.LuisSubscriptionKey);
            _luisClient = new LUISRuntimeClient(credentials)
            {
                Endpoint = _config.LuisEndpoint
            };
        }

        public async Task<SpeechRecognizedResult> ValidateSpeechInputAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new SpeechRecognizedResult();
            }

            PredictionResponse prediction = null;
            SpeechOutputResult speechOutputResult = new();
            try
            {
                prediction = await _luisClient.Prediction.GetSlotPredictionAsync(Guid.Parse(_config.LuisAppId), _config.LuisAppSlot,
                    new PredictionRequest
                    {
                        Query = text
                    });

                speechOutputResult = await AnalyzePredictionAsync(prediction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during speech recognition");
            }

            return new SpeechRecognizedResult
            {
                Speaker = null,
                Text = text,
                TopIntent = prediction?.Prediction?.TopIntent,
                Intents = prediction?.Prediction?.Intents,
                Entities = prediction?.Prediction?.Entities,
                VoiceResponse = speechOutputResult.Output
            };
        }

        private async Task<SpeechOutputResult> AnalyzePredictionAsync(PredictionResponse predictionResponse)
        {
            switch (predictionResponse.Prediction.TopIntent)
            {
                case "Hue.TurnOn":
                    await _intentExecutor.Handle(new HueTurnOn(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Hue.TurnOff":
                    await _intentExecutor.Handle(new HueTurnOff(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Hue.DisplayDetails":
                    await _intentExecutor.Handle<HueState>(new HueDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Hue.HideDetails":
                    await _intentExecutor.Handle<HueState>(new HueDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Hue.Show":
                    await _intentExecutor.Handle<HueState>(new HueShow(true));
                    return new SpeechOutputResult();
                case "Hue.Hide":
                    await _intentExecutor.Handle<HueState>(new HueShow(false));
                    return new SpeechOutputResult();
                case "Weather.CheckWeatherValue":
                case "Weather.QueryWeather":
                    {
                        OneCallWeatherForecast forecast = await _intentExecutor.Handle(new WeatherQueryWeather(predictionResponse.Prediction.Entities));
                        return new SpeechOutputResult($"Heute gibt es in Wendlingen {forecast.Daily[0].Temp.Day} Grad und es ist {forecast.Daily[0].Weather[0].Description}.");
                    }
                case "Weather.Show":
                    await _intentExecutor.Handle<WeatherState>(new WeatherShow(true));
                    return new SpeechOutputResult();
                case "Weather.Hide":
                    await _intentExecutor.Handle<WeatherState>(new WeatherShow(false));
                    return new SpeechOutputResult();
                case "Weather.DisplayForecast":
                    await _intentExecutor.Handle<WeatherState>(new WeatherDisplayForecast(true));
                    return new SpeechOutputResult();
                case "Weather.HideForecast":
                    await _intentExecutor.Handle<WeatherState>(new WeatherDisplayForecast(false));
                    return new SpeechOutputResult();
                case "Bring.AddToDo":
                    await _intentExecutor.Handle(new BringAddToDo(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Bring.DeleteToDo":
                    await _intentExecutor.Handle(new BringDeleteToDo(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Bring.DisplayDetails":
                    await _intentExecutor.Handle<BringState>(new BringDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Bring.Show":
                    await _intentExecutor.Handle<BringState>(new BringShow(true));
                    return new SpeechOutputResult();
                case "Bring.Hide":
                    await _intentExecutor.Handle<BringState>(new BringShow(false));
                    return new SpeechOutputResult();
                case "Bring.HideDetails":
                    await _intentExecutor.Handle<BringState>(new BringDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Routes.GetRoute":
                    (RouteResponse route, GeosearchResponse source, GeosearchResponse destination) = await _intentExecutor.Handle(new RoutesGetRoute(predictionResponse.Prediction.Entities));
                        return new SpeechOutputResult("Route gefunden");
                case "Routes.DisplayDetails":
                    await _intentExecutor.Handle<RouteState>(new RoutesDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Routes.HideDetails":
                    await _intentExecutor.Handle<RouteState>(new RoutesDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Routes.Show":
                    await _intentExecutor.Handle<RouteState>(new RoutesShow(true));
                    return new SpeechOutputResult();
                case "Routes.Hide":
                    await _intentExecutor.Handle<RouteState>(new RoutesShow(false));
                    return new SpeechOutputResult();
                case "Spotify.NextSong":
                    await _intentExecutor.Handle(new SpotifyNextSong());
                    return new SpeechOutputResult();
                case "Spotify.Show":
                    await _intentExecutor.Handle<SpotifyState>(new SpotifyShow(true));
                    return new SpeechOutputResult();
                case "Spotify.Hide":
                    await _intentExecutor.Handle<SpotifyState>(new SpotifyShow(false));
                    return new SpeechOutputResult();
                case "Calendar.DisplayDays":
                    await _intentExecutor.Handle(new CalendarDisplayDays(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Calendar.Show":
                    await _intentExecutor.Handle<CalendarState>(new CalendarShow(true));
                    return new SpeechOutputResult();
                case "Calendar.Hide":
                    await _intentExecutor.Handle<CalendarState>(new CalendarShow(false));
                    return new SpeechOutputResult();
                case "Fuel.Show":
                    await _intentExecutor.Handle<FuelState>(new FuelShow(true));
                    return new SpeechOutputResult();
                case "Fuel.Hide":
                    await _intentExecutor.Handle<FuelState>(new FuelShow(false));
                    return new SpeechOutputResult();
                case "Fuel.Refresh":
                    await _intentExecutor.Handle(new FuelRefresh());
                    return new SpeechOutputResult();
                case "Fuel.DisplayDetails":
                    await _intentExecutor.Handle<FuelState>(new FuelDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Fuel.HideDetails":
                    await _intentExecutor.Handle<FuelState>(new FuelDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Clock.Timer":
                    await _intentExecutor.Handle(new ClockTimer(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult("Timer läuft");
                case "Clock.Show":
                    await _intentExecutor.Handle(new ClockShow(true));
                    return new SpeechOutputResult();
                case "Clock.Hide":
                    await _intentExecutor.Handle(new ClockShow(false));
                    return new SpeechOutputResult();
                case "Clock.Stoptimer":
                    await _intentExecutor.Handle(new StopClockTimer());
                    return new SpeechOutputResult();
                case "Vvs.Show":
                    await _intentExecutor.Handle<VvsState>(new VvsShow(true));
                    return new SpeechOutputResult();
                case "Vvs.Hide":
                    await _intentExecutor.Handle<VvsState>(new VvsShow(false));
                    return new SpeechOutputResult();
                case "Fitbit.Show":
                    await _intentExecutor.Handle<FitbitState>(new FitbitShow(true));
                    return new SpeechOutputResult();
                case "Fitbit.Hide":
                    await _intentExecutor.Handle<FitbitState>(new FitbitShow(false));
                    return new SpeechOutputResult();
                case "Googlefit.Show":
                    await _intentExecutor.Handle<GoogleFitState>(new GoogleFitShow(true));
                    return new SpeechOutputResult();
                case "Googlefit.Hide":
                    await _intentExecutor.Handle<GoogleFitState>(new GoogleFitShow(false));
                    return new SpeechOutputResult();
                case "Soccer.Show":
                    await _intentExecutor.Handle<SoccerState>(new SoccerShow(true));
                    return new SpeechOutputResult();
                case "Soccer.Hide":
                    await _intentExecutor.Handle<SoccerState>(new SoccerShow(false));
                    return new SpeechOutputResult();
                case "News.Show":
                    await _intentExecutor.Handle<NewsState>(new NewsShow(true));
                    return new SpeechOutputResult();
                case "News.Hide":
                    await _intentExecutor.Handle<NewsState>(new NewsShow(false));
                    return new SpeechOutputResult();
                case "Mirror.Show":
                    await _intentExecutor.Handle(new MirrorShow(true));
                    return new SpeechOutputResult();
                default:
                    return new SpeechOutputResult();
            };
        }
    }
}
