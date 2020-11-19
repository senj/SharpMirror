namespace SmartMirror.Data.Speech
{
    public class SpeechOutputResult
    {
        public SpeechOutputResult()
        {
            Output = string.Empty;
        }

        public SpeechOutputResult(string output)
        {
            Output = output;
        }

        public string Output { get; set; }
    }
}

