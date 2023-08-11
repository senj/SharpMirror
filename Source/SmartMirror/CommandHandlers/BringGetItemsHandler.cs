using MediatR;
using SmartMirror.Commands;
using SmartMirror.Data.Bring;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.CommandHandlers
{
    public class BringGetItemsHandler : IRequestHandler<BringGetItems, BringItemResponse>
    {
        private readonly BringService _bringService;

        public BringGetItemsHandler(BringService bringService)
        {
            _bringService = bringService;
        }

        public Task<BringItemResponse> Handle(BringGetItems message, CancellationToken cancellationToken)
        {
            return _bringService.GetItemsAsync(message.LoadFromCache);
        }
    }
}
