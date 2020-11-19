using System.Collections.Generic;

namespace SmartMirror.Data.Speech
{
    public class SpeechRecognitionConfiguration
    {
        public string SpeechApiSubscriptionKey { get; set; }

        public string SpeechApiRegion { get; set; }

        public string SpeechApiTargetLanguage { get; set; }

        public Dictionary<string, string> SpeechApiVoiceProfileMapping { get; set; }

        public string LuisAppId { get; set; }

        public string LuisAppSlot { get; set; }

        public string LuisSubscriptionKey { get; set; }

        public string LuisEndpoint { get; set; }
    }
}
