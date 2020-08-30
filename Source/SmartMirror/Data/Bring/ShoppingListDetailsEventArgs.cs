namespace SmartMirror.Data.Bring
{
    public class ShoppingListDetailsEventArgs
    {
        public ShoppingListDetailsEventArgs(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; set; }
    }
}
