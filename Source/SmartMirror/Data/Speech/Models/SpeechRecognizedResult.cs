using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System.Collections.Generic;

namespace SmartMirror.Data.Speech
{
    public class SpeechRecognizedResult
    {
        public IDictionary<string, Intent> Intents { get; set; }

        public string TopIntent { get; set; }

        public string Text { get; set; }

        public IDictionary<string, object> Entities { get; set; }

        public string Speaker { get; set; }

        public string VoiceResponse { get; set; }
    }
}
