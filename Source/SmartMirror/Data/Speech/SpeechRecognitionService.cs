using MediatR;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class SpeechRecognitionService
    {
        private readonly ILogger<SpeechRecognitionService> _logger;
        private readonly IMediator _mediator;
        private readonly SpeechRecognitionConfiguration _config;
        private readonly SpeechConfig _speechConfiguration;
        private readonly LUISRuntimeClient _luisClient;
        private readonly SpeechRecognizer _recognizer;
        private readonly List<VoiceProfile> _voiceProfiles;
        private readonly Dictionary<string, string> _profileMapping;
        private string _speaker;

        public event EventHandler<SpeechRecognizedEventArgs> SpeechRecognized;
        public event EventHandler<SpeechStartedEventArgs> SpeechStarted;
        public event EventHandler<SpeechEndedEventArgs> SpeechEnded;
        public event EventHandler<SpeechOutputEventArgs> SpeechOutputRequested;

        public SpeechRecognitionService(
            ILogger<SpeechRecognitionService> logger,
            IOptions<SpeechRecognitionConfiguration> speechRecognitionConfiguration,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _config = speechRecognitionConfiguration.Value;
            _speechConfiguration = SpeechConfig.FromSubscription(_config.SpeechApiSubscriptionKey, _config.SpeechApiRegion);

            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(_config.LuisSubscriptionKey);
            _luisClient = new LUISRuntimeClient(credentials)
            {
                Endpoint = _config.LuisEndpoint
            };

            _recognizer = new SpeechRecognizer(_speechConfiguration, _config.SpeechApiTargetLanguage);
            _recognizer.Recognized += Recognizer_Recognized;
            _recognizer.SpeechStartDetected += Recognizer_SpeechStartDetected;
            _recognizer.SpeechEndDetected += Recognizer_SpeechEndDetected;

            _voiceProfiles = new List<VoiceProfile>();
            _profileMapping = new Dictionary<string, string>();
            foreach (var entry in _config.SpeechApiVoiceProfileMapping)
            {
                _voiceProfiles.Add(new VoiceProfile(entry.Key));
                _profileMapping.Add(entry.Key, entry.Value);
            }

            Task.Run(() => StartRecognizer());
        }

        public async Task StartRecognizer()
        {
            var stopRecognition = new TaskCompletionSource<int>();
            await _recognizer.StartContinuousRecognitionAsync();

            Task.WaitAny(new[] { stopRecognition.Task });

            await _recognizer.StopContinuousRecognitionAsync();
        }

        public void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Result.Text))
            {
                return;
            }

            PredictionResponse prediction = null;
            try
            {
                prediction = _luisClient.Prediction.GetSlotPredictionAsync(Guid.Parse(_config.LuisAppId), _config.LuisAppSlot,
                    new PredictionRequest
                    {
                        Query = e.Result.Text
                    }).GetAwaiter().GetResult();

                AnalyzePredictionAsync(prediction).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during speech recognition");
            }
            
            SpeechRecognized?.Invoke(this, new SpeechRecognizedEventArgs
            {
                Speaker = _speaker ?? null,
                Text = e.Result.Text,
                TopIntent = prediction?.Prediction?.TopIntent,
                Intents = prediction?.Prediction?.Intents,
                Entities = prediction?.Prediction?.Entities
            });
        }

        public void Recognizer_SpeechEndDetected(object sender, RecognitionEventArgs e)
        {
            SpeechEnded?.Invoke(this, new SpeechEndedEventArgs());
        }

        public void Recognizer_SpeechStartDetected(object sender, RecognitionEventArgs e)
        {
            SpeechStarted?.Invoke(this, new SpeechStartedEventArgs());
            Task.Run(async () => await SpeakerIdentification()).ContinueWith(p => _speaker = p.Result);
        }

        public async Task<string> SpeakerIdentification()
        {
            var model = SpeakerIdentificationModel.FromProfiles(_voiceProfiles);

            var speakerRecognizer = new SpeakerRecognizer(_speechConfiguration, AudioConfig.FromDefaultMicrophoneInput());
            var result = await speakerRecognizer.RecognizeOnceAsync(model);

            if (result.Reason == ResultReason.Canceled)
            {
                var cancelled = SpeakerRecognitionCancellationDetails.FromResult(result);
                _logger.LogError("Speaker recognizer was canceled: {@error}", cancelled);
            }

            if (string.IsNullOrEmpty(result.ProfileId) || result.ProfileId == Guid.Empty.ToString())
            {
                return string.Empty;
            }

            _logger.LogInformation($"The most similiar voice profile is {_profileMapping[result.ProfileId]} with similiarity score {result.Score}");
            return _profileMapping[result.ProfileId];
        }

        private async Task AnalyzePredictionAsync(PredictionResponse predictionResponse)
        {
            switch (predictionResponse.Prediction.TopIntent)
            {
                case "HomeAutomation.TurnOn":
                    await _mediator.Publish(new TurnOn(predictionResponse.Prediction.Entities));
                    break;
                case "HomeAutomation.TurnOff":
                    await _mediator.Publish(new TurnOff(predictionResponse.Prediction.Entities));
                    break;
                case "Weather.CheckWeatherValue":
                case "Weather.QueryWeather":
                    {
                        var forecast = await _mediator.Send(new WeatherInformationRequest(true));
                        SpeechOutputRequested?.Invoke(this, new SpeechOutputEventArgs($"Heute gibt es in Wendlingen {forecast.Daily[0].Temp.Day} Grad und es ist {forecast.Daily[0].Weather[0].Description}."));
                    }
                    break;
                case "Weather.DisplayForecast":
                    await _mediator.Publish(new WeatherDisplayType(true));
                    break;
                case "Weather.HideForecast":
                    await _mediator.Publish(new WeatherDisplayType(false));
                    break;
                case "ToDo.AddToDo":
                    await _mediator.Publish(new AddListEntry(predictionResponse.Prediction.Entities));
                    break;
                case "ToDo.DisplayDetails":
                    await _mediator.Publish(new ShoppingListDisplayType(true));
                    break;
                case "ToDo.HideDetails":
                    await _mediator.Publish(new ShoppingListDisplayType(false));
                    break;
                case "Places.GetRoute":
                    {
                        var routeResponse = await _mediator.Send(new GetDistanceRequest(predictionResponse.Prediction.Entities));
                        SpeechOutputRequested?.Invoke(this, new SpeechOutputEventArgs("Route gefunden"));
                    }
                    break;
            };
        }
    }
}
