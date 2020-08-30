using MediatR;
using SmartMirror.Data.Bring;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class ShoppingListDisplayTypeNotificationHandler : INotificationHandler<ShoppingListDisplayType>
    {
        private readonly BringService _bringService;

        public ShoppingListDisplayTypeNotificationHandler(BringService bringService)
        {
            _bringService = bringService;
        }

        public Task Handle(ShoppingListDisplayType request, CancellationToken cancellationToken)
        {
            _bringService.SetShoppingListDisplayType(request.ShowDetails);
            return Task.CompletedTask;
        }
    }
}
