namespace SmartMirror.Notifications
{
    public class ShoppingListDisplayType
    {
        public ShoppingListDisplayType(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; }
    }
}
