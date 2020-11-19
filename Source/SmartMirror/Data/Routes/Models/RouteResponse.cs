using System;

namespace SmartMirror.Data.Routes
{
    public class RouteResponse
    {
        public RouteResponse()
        {
            routes = new Route[] { };
        }

        public string formatVersion { get; set; }
        public Route[] routes { get; set; }
    }

    public class Route
    {
        public RouteSummary summary { get; set; }
        public Leg[] legs { get; set; }
        public Section[] sections { get; set; }
    }

    public class RouteSummary
    {
        public int lengthInMeters { get; set; }
        public int travelTimeInSeconds { get; set; }
        public int trafficDelayInSeconds { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
        public int noTrafficTravelTimeInSeconds { get; set; }
        public int historicTrafficTravelTimeInSeconds { get; set; }
        public int liveTrafficIncidentsTravelTimeInSeconds { get; set; }
    }

    public class Leg
    {
        public LegSummary summary { get; set; }
        public Point[] points { get; set; }
    }

    public class LegSummary
    {
        public int lengthInMeters { get; set; }
        public int travelTimeInSeconds { get; set; }
        public int trafficDelayInSeconds { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
        public int noTrafficTravelTimeInSeconds { get; set; }
        public int historicTrafficTravelTimeInSeconds { get; set; }
        public int liveTrafficIncidentsTravelTimeInSeconds { get; set; }
    }

    public class Point
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Section
    {
        public int startPointIndex { get; set; }
        public int endPointIndex { get; set; }
        public string sectionType { get; set; }
        public string travelMode { get; set; }
    }
}
