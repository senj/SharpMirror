namespace SmartMirror.Data.Routes
{
    public class RouteRequest
    {
        public Geopoint Departure { get; set; }

        public Geopoint Destination { get; set; }
    }

    public class Geopoint 
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}