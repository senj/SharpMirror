using MediatR;
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
        private readonly IMediator _mediator;
        private readonly SpeechRecognitionConfiguration _config;
        private readonly LUISRuntimeClient _luisClient;
        
        public SpeechRecognitionService(
            ILogger<SpeechRecognitionService> logger,
            IOptions<SpeechRecognitionConfiguration> speechRecognitionConfiguration,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _config = speechRecognitionConfiguration.Value;
            
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(_config.LuisSubscriptionKey);
            _luisClient = new LUISRuntimeClient(credentials)
            {
                Endpoint = _config.LuisEndpoint
            };
        }

        public SpeechRecognizedResult ValidateSpeechInput(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new SpeechRecognizedResult();
            }

            PredictionResponse prediction = null;
            try
            {
                prediction = _luisClient.Prediction.GetSlotPredictionAsync(Guid.Parse(_config.LuisAppId), _config.LuisAppSlot,
                    new PredictionRequest
                    {
                        Query = text
                    }).GetAwaiter().GetResult();

                AnalyzePredictionAsync(prediction).GetAwaiter().GetResult();
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
                Entities = prediction?.Prediction?.Entities
            };
        }

        private async Task<SpeechOutputResult> AnalyzePredictionAsync(PredictionResponse predictionResponse)
        {
            switch (predictionResponse.Prediction.TopIntent)
            {
                case "HomeAutomation.TurnOn":
                    await _mediator.Publish(new TurnOn(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "HomeAutomation.TurnOff":
                    await _mediator.Publish(new TurnOff(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "Weather.CheckWeatherValue":
                case "Weather.QueryWeather":
                    {
                        var forecast = await _mediator.Send(new WeatherInformationRequest(true));
                        return new SpeechOutputResult($"Heute gibt es in Wendlingen {forecast.Daily[0].Temp.Day} Grad und es ist {forecast.Daily[0].Weather[0].Description}.");
                    }
                case "Weather.DisplayForecast":
                    await _mediator.Publish(new WeatherDisplayType(true));
                    return new SpeechOutputResult();
                case "Weather.HideForecast":
                    await _mediator.Publish(new WeatherDisplayType(false));
                    return new SpeechOutputResult();
                case "ToDo.AddToDo":
                    await _mediator.Publish(new AddListEntry(predictionResponse.Prediction.Entities));
                    return new SpeechOutputResult();
                case "ToDo.DisplayDetails":
                    await _mediator.Publish(new ShoppingListDisplayType(true));
                    return new SpeechOutputResult();
                case "ToDo.HideDetails":
                    await _mediator.Publish(new ShoppingListDisplayType(false));
                    return new SpeechOutputResult();
                case "Places.GetRoute":
                    {
                        var routeResponse = await _mediator.Send(new GetDistanceRequest(predictionResponse.Prediction.Entities));
                        return new SpeechOutputResult("Route gefunden");
                    }
                case "Spotify.NextSong":
                    await _mediator.Publish(new NextSongRequested());
                    return new SpeechOutputResult();
                default:
                    return new SpeechOutputResult();
            };
        }
    }
}
