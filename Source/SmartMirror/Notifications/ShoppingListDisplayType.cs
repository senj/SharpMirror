using MediatR;

namespace SmartMirror.Notifications
{
    public class ShoppingListDisplayType : INotification
    {
        public ShoppingListDisplayType(bool showDetails)
        {
            ShowDetails = showDetails;
        }

        public bool ShowDetails { get; }
    }
}
