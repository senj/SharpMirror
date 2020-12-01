namespace SmartMirror.Intents
{
    public class HueShow
    {
        public HueShow(bool displayHue)
        {
            DisplayHue = displayHue;
        }

        public bool DisplayHue { get; }
    }
}
