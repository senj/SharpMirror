namespace SmartMirror.Data.Speech
{
    public class SpeechOutputResult
    {
        public SpeechOutputResult()
        {
        }

        public SpeechOutputResult(string output)
        {
            Output = output;
        }

        public string Output { get; set; }
    }
}

