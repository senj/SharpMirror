namespace SmartMirror.Intents
{
    public class FuelDisplayDetails
    {
        public FuelDisplayDetails(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; }
    }
}
