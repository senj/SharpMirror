using MediatR;
using SmartMirror.Data.Routes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class GetDistanceRequestHandler : IRequestHandler<GetDistanceRequest, RouteResponse>
    {
        private readonly RouteService _routeService;

        public GetDistanceRequestHandler(RouteService routeService)
        {
            _routeService = routeService;
        }

        public async Task<RouteResponse> Handle(GetDistanceRequest request, CancellationToken cancellationToken)
        {
            GeosearchResponse source = await _routeService.SearchAsync(request.Source);
            GeosearchResponse destination = await _routeService.SearchAsync(request.Destination);

            if (source != null && destination != null)
            {
                RouteResponse routeResponse = await _routeService.GetRouteAsync(new RouteRequest
                {
                    Departure = new Geopoint
                    {
                        Latitude = source.results.FirstOrDefault().position.lat,
                        Longitude = source.results.FirstOrDefault().position.lon
                    },
                    Destination = new Geopoint
                    {
                        Latitude = destination.results.FirstOrDefault().position.lat,
                        Longitude = destination.results.FirstOrDefault().position.lon
                    }
                });

                _routeService.DisplayRoute(source, destination, routeResponse);
            }

            return new RouteResponse();
        }
    }
}
