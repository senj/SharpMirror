namespace SmartMirror.Intents
{
    public class ClockShow
    {
        public ClockShow(bool displayClock)
        {
            DisplayClock = displayClock;
        }

        public bool DisplayClock { get; }
    }
}
