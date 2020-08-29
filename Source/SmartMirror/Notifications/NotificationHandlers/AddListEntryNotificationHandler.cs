using MediatR;
using SmartMirror.Data.Bring;
using SmartMirror.Data.Speech;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class AddListEntryNotificationHandler : INotificationHandler<AddListEntry>
    {
        private readonly BringService _bringService;

        public AddListEntryNotificationHandler(BringService bringService)
        {
            _bringService = bringService;
        }

        public async Task Handle(AddListEntry request, CancellationToken cancellationToken)
        {
            foreach (string entry in request.ItemNames)
            {
                await _bringService.AddItemAsync(entry, request.Details);
            }

            if (request.ItemNames.Any())
            {
                await _bringService.GetItemsAsync(false);
            }
        }
    }
}
