namespace SmartMirror.Notifications
{
    public class RoutesDisplayType
    {
        public RoutesDisplayType(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; }
    }
}
