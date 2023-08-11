using MediatR;
using SmartMirror.Data.Bring;

namespace SmartMirror.Commands
{
    public record BringGetItems : IRequest<BringItemResponse> 
    {
        public bool LoadFromCache { get; init; }
    }
}
