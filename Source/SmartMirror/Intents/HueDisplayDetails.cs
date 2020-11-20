namespace SmartMirror.Intents
{
    public class HueDisplayDetails
    {
        public HueDisplayDetails(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; }
    }
}
