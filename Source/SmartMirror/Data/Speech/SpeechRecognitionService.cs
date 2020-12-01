using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Intents;
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

            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(_config.LuisSubscriptionKey);
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
            SpeechOutputResult speechOutputResult = new SpeechOutputResult();
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
                    await _intentExecutor.Handle(new HueDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Hue.HideDetails":
                    await _intentExecutor.Handle(new HueDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Weather.CheckWeatherValue":
                case "Weather.QueryWeather":
                    {
                        var forecast = await _intentExecutor.Handle(new WeatherQueryWeather(predictionResponse.Prediction.Entities));
                        return new SpeechOutputResult($"Heute gibt es in Wendlingen {forecast.Daily[0].Temp.Day} Grad und es ist {forecast.Daily[0].Weather[0].Description}.");
                    }
                case "Weather.Show":
                    await _intentExecutor.Handle(new WeatherShow(true));
                    return new SpeechOutputResult();
                case "Weather.Hide":
                    await _intentExecutor.Handle(new WeatherShow(false));
                    return new SpeechOutputResult();
                case "Weather.DisplayForecast":
                    await _intentExecutor.Handle(new WeatherDisplayForecast(true));
                    return new SpeechOutputResult();
                case "Weather.HideForecast":
                    await _intentExecutor.Handle(new WeatherDisplayForecast(false));
                    return new SpeechOutputResult();
                case "Bring.AddToDo":
                    await _intentExecutor.Handle(new BringAddToDo(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Bring.DeleteToDo":
                    await _intentExecutor.Handle(new BringDeleteToDo(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Bring.DisplayDetails":
                    await _intentExecutor.Handle(new BringDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Bring.Show":
                    await _intentExecutor.Handle(new BringShow(true));
                    return new SpeechOutputResult();
                case "Bring.Hide":
                    await _intentExecutor.Handle(new BringShow(false));
                    return new SpeechOutputResult();
                case "Bring.HideDetails":
                    await _intentExecutor.Handle(new BringDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Routes.GetRoute":
                        var routeResponse = await _intentExecutor.Handle(new RoutesGetRoute(predictionResponse.Prediction.Entities));
                        return new SpeechOutputResult("Route gefunden");
                case "Routes.DisplayDetails":
                    await _intentExecutor.Handle(new RoutesDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Routes.HideDetails":
                    await _intentExecutor.Handle(new RoutesDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Spotify.NextSong":
                    await _intentExecutor.Handle(new SpotifyNextSong());
                    return new SpeechOutputResult();
                case "Spotify.Show":
                    await _intentExecutor.Handle(new SpotifyShow(true));
                    return new SpeechOutputResult();
                case "Spotify.Hide":
                    await _intentExecutor.Handle(new SpotifyShow(false));
                    return new SpeechOutputResult();
                case "Calendar.DisplayDays":
                    await _intentExecutor.Handle(new CalendarDisplayDays(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Calendar.Show":
                    await _intentExecutor.Handle(new CalendarShow(true));
                    return new SpeechOutputResult();
                case "Calendar.Hide":
                    await _intentExecutor.Handle(new CalendarShow(false));
                    return new SpeechOutputResult();
                case "Fuel.Show":
                    await _intentExecutor.Handle(new FuelShow(true));
                    return new SpeechOutputResult();
                case "Fuel.Hide":
                    await _intentExecutor.Handle(new FuelShow(false));
                    return new SpeechOutputResult();
                case "Fuel.Refresh":
                    await _intentExecutor.Handle(new FuelRefresh());
                    return new SpeechOutputResult();
                case "Fuel.DisplayDetails":
                    await _intentExecutor.Handle(new FuelDisplayDetails(true));
                    return new SpeechOutputResult();
                case "Fuel.HideDetails":
                    await _intentExecutor.Handle(new FuelDisplayDetails(false));
                    return new SpeechOutputResult();
                case "Clock.Timer":
                    await _intentExecutor.Handle(new ClockTimer(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult("Timer läuft");
                case "Mirror.Show":
                    await _intentExecutor.Handle(new MirrorShow(true));
                    return new SpeechOutputResult();
                default:
                    return new SpeechOutputResult();
            };
        }
    }
}
