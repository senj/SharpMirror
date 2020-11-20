namespace SmartMirror.Intents
{
    public class RoutesDisplayDetails
    {
        public RoutesDisplayDetails(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; }
    }
}
