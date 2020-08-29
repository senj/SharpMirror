namespace SmartMirror.Data.Speech
{
    public class SpeechOutputEventArgs
    {
        public SpeechOutputEventArgs(string output)
        {
            Output = output;
        }

        public string Output { get; set; }
    }
}

