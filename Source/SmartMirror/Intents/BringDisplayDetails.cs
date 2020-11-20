namespace SmartMirror.Intents
{
    public class BringDisplayDetails
    {
        public BringDisplayDetails(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; }
    }
}
