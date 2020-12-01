using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.Routes
{
    public class RouteState : Displayable
    {
        private readonly RouteService _routeService;

        public RouteState(RouteService routeService)
        {
            _routeService = routeService;
        }

        public RouteResponse RouteResponse { get; set; }

        public GeosearchResponse Destination { get; set; }

        public GeosearchResponse Source { get; set; }

        public async Task<(RouteResponse route, GeosearchResponse source, GeosearchResponse destination)> FindRouteAsync(string source, string destination)
        {
            (RouteResponse, Source, Destination) = await _routeService.FindRouteAsync(source, destination);
            RaiseOnChangeEvent();

            return (RouteResponse, Source, Destination);
        }
    }
}