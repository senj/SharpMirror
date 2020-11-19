namespace SmartMirror.Data.Routes
{

    public class GeosearchResponse
    {
        public GeosearchSummary summary { get; set; }
        public Result[] results { get; set; }
    }

    public class GeosearchSummary
    {
        public string query { get; set; }
        public string queryType { get; set; }
        public int queryTime { get; set; }
        public int numResults { get; set; }
        public int offset { get; set; }
        public int totalResults { get; set; }
        public int fuzzyLevel { get; set; }
    }

    public class Result
    {
        public string type { get; set; }
        public string id { get; set; }
        public float score { get; set; }
        public string entityType { get; set; }
        public Address address { get; set; }
        public Position position { get; set; }
        public Viewport viewport { get; set; }
        public Boundingbox boundingBox { get; set; }
        public Datasources dataSources { get; set; }
    }

    public class Address
    {
        public string municipality { get; set; }
        public string countrySecondarySubdivision { get; set; }
        public string countrySubdivision { get; set; }
        public string countryCode { get; set; }
        public string country { get; set; }
        public string countryCodeISO3 { get; set; }
        public string freeformAddress { get; set; }
        public string streetName { get; set; }
        public string postalCode { get; set; }
        public string localName { get; set; }
        public string municipalitySubdivision { get; set; }
    }

    public class Position
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class Viewport
    {
        public Topleftpoint topLeftPoint { get; set; }
        public Btmrightpoint btmRightPoint { get; set; }
    }

    public class Topleftpoint
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class Btmrightpoint
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class Boundingbox
    {
        public Topleftpoint1 topLeftPoint { get; set; }
        public Btmrightpoint1 btmRightPoint { get; set; }
    }

    public class Topleftpoint1
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class Btmrightpoint1
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class Datasources
    {
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public string id { get; set; }
    }

}