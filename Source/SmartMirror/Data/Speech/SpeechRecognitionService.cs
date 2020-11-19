using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Notifications;
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
                case "HomeAutomation.TurnOn":
                    await _intentExecutor.Handle(new TurnOn(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "HomeAutomation.TurnOff":
                    await _intentExecutor.Handle(new TurnOff(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Weather.CheckWeatherValue":
                case "Weather.QueryWeather":
                    {
                        var forecast = await _intentExecutor.Handle(new WeatherInformationRequest(true));
                        return new SpeechOutputResult($"Heute gibt es in Wendlingen {forecast.Daily[0].Temp.Day} Grad und es ist {forecast.Daily[0].Weather[0].Description}.");
                    }
                case "Weather.DisplayForecast":
                    await _intentExecutor.Handle(new WeatherDisplayType(true));
                    return new SpeechOutputResult();
                case "Weather.HideForecast":
                    await _intentExecutor.Handle(new WeatherDisplayType(false));
                    return new SpeechOutputResult();
                case "ToDo.AddToDo":
                    await _intentExecutor.Handle(new AddListEntry(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "ToDo.DeleteToDo":
                    await _intentExecutor.Handle(new RemoveListEntry(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "ToDo.DisplayDetails":
                    await _intentExecutor.Handle(new ShoppingListDisplayType(true));
                    return new SpeechOutputResult();
                case "ToDo.HideDetails":
                    await _intentExecutor.Handle(new ShoppingListDisplayType(false));
                    return new SpeechOutputResult();
                case "Places.GetRoute":
                        var routeResponse = await _intentExecutor.Handle(new GetDistanceRequest(predictionResponse.Prediction.Entities));
                        return new SpeechOutputResult("Route gefunden");
                case "Places.DisplayDetails":
                    await _intentExecutor.Handle(new RoutesDisplayType(true));
                    return new SpeechOutputResult();
                case "Places.HideDetails":
                    await _intentExecutor.Handle(new RoutesDisplayType(false));
                    return new SpeechOutputResult();
                case "Spotify.NextSong":
                    await _intentExecutor.Handle(new NextSongRequested());
                    return new SpeechOutputResult();
                default:
                    return new SpeechOutputResult();
            };
        }
    }
}
