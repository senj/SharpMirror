namespace SmartMirror.Data.Routes
{
    public class DisplayRouteEventArgs
    {
        public RouteResponse RouteResponse { get; set; }

        public GeosearchResponse Destination { get; set; }

        public GeosearchResponse Source { get; set; }
    }
}