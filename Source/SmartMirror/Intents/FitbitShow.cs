namespace SmartMirror.Intents
{
    public class FitbitShow
    {
        public FitbitShow(bool displayFitbit)
        {
            DisplayFitbit = displayFitbit;
        }

        public bool DisplayFitbit { get; }
    }
}
