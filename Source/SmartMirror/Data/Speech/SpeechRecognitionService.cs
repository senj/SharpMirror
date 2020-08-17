using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.Speech
{
    public class SpeechRecognitionService
    {
        private readonly ILogger<SpeechRecognitionService> _logger;
        private readonly SpeechConfig _config;
        private readonly SpeechRecognizer _recognizer;
        private readonly List<VoiceProfile> _voiceProfiles;
        private readonly Dictionary<string, string> _profileMapping;

        public SpeechRecognitionService(
            ILogger<SpeechRecognitionService> logger)
        {
            _logger = logger;

            _config =
                SpeechConfig.FromSubscription(
                    "d30127fd27014d248a0e58a1435f73af",
                    "westus");

            _recognizer = new SpeechRecognizer(_config, "de-DE");
            
            var voiceProfileLukas = new VoiceProfile("dbdee46b-ce33-416d-82ea-6224dbcb484b");
            var voiceProfileTest = new VoiceProfile("c1271d36-3764-4420-979f-13adf7748d66");

            _voiceProfiles = new List<VoiceProfile>();
            _profileMapping = new Dictionary<string, string>();

            _voiceProfiles.Add(voiceProfileLukas);
            _profileMapping.Add("dbdee46b-ce33-416d-82ea-6224dbcb484b", "lukas");

            _voiceProfiles.Add(voiceProfileTest);
            _profileMapping.Add("c1271d36-3764-4420-979f-13adf7748d66", "test");
        }

        public SpeechRecognizer GetRecognizer()
        {
            return _recognizer;
        }

        public async Task<string> SpeakerVerify()
        {
            var model = SpeakerIdentificationModel.FromProfiles(_voiceProfiles);

            var speakerRecognizer = new SpeakerRecognizer(_config, AudioConfig.FromDefaultMicrophoneInput());
            var result = await speakerRecognizer.RecognizeOnceAsync(model);

            if (result.Reason == ResultReason.Canceled)
            {
                var cancelled = SpeakerRecognitionCancellationDetails.FromResult(result);
            }

            if (string.IsNullOrEmpty(result.ProfileId) || result.ProfileId == Guid.Empty.ToString())
            {
                return string.Empty;
            }

            _logger.LogInformation($"The most similiar voice profile is {_profileMapping[result.ProfileId]} with similiarity score {result.Score}");
            return _profileMapping[result.ProfileId];
        }
    }
}
