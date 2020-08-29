using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class SpeechRecognitionService
    {
        private readonly ILogger<SpeechRecognitionService> _logger;
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

        public SpeechRecognitionService(
            ILogger<SpeechRecognitionService> logger,
            IOptions<SpeechRecognitionConfiguration> speechRecognitionConfiguration)
        {
            _logger = logger;
            _config = speechRecognitionConfiguration.Value;
            _speechConfiguration = SpeechConfig.FromSubscription(_config.SpeechApiSubscriptionKey, _config.SpeechApiRegion);

            var credentials = new ApiKeyServiceClientCredentials(_config.LuisSubscriptionKey);
            _luisClient = new LUISRuntimeClient(credentials) 
            {
                Endpoint = _config.LuisEndpoint
            };
            
            _recognizer = new SpeechRecognizer(_speechConfiguration, _config.SpeechApiTargetLanguage);
            _recognizer.Recognized += Recognizer_Recognized;
            _recognizer.Recognizing += Recognizer_Recognizing;

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

        internal void Recognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
        {
        }

        internal void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Result.Text))
            {
                return;
            }

            var prediction = _luisClient.Prediction.GetSlotPredictionAsync(Guid.Parse(_config.LuisAppId), _config.LuisAppSlot,
                    new PredictionRequest
                    {
                        Query = e.Result.Text
                    }).GetAwaiter().GetResult();

            SpeechRecognized?.Invoke(this, new SpeechRecognizedEventArgs 
            {
                Speaker = _speaker ?? null,
                Text = e.Result.Text,
                TopIntent = prediction?.Prediction?.TopIntent,
                Intents = prediction?.Prediction?.Intents,
                Entities = prediction?.Prediction?.Entities
            });
        }

        internal void Recognizer_SpeechEndDetected(object sender, RecognitionEventArgs e)
        {
            SpeechEnded?.Invoke(this, new SpeechEndedEventArgs());
        }

        internal void Recognizer_SpeechStartDetected(object sender, RecognitionEventArgs e)
        {
            SpeechStarted?.Invoke(this, new SpeechStartedEventArgs());
            Task.Run(async () => await SpeakerVerify()).ContinueWith(p => _speaker = p.Result);
        }

        public async Task<string> SpeakerVerify()
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
    }

    public class SpeechEndedEventArgs
    {
    }

    public class SpeechStartedEventArgs
    {
    }

    public class SpeechRecognizedEventArgs
    {
        public IDictionary<string, Intent> Intents { get; set; }

        public string TopIntent { get; set; }

        public string Text { get; set; }

        public IDictionary<string, object> Entities { get; set; }

        public string Speaker { get; set; }
    }
}
