using System;

namespace SmartMirror.Data.VVS
{
    public class VvsResponse
    {
        public object[] SystemMessages { get; set; }
        public Stopevent[] StopEvents { get; set; }
    }

    public class Stopevent
    {
        public bool IsRealtimeControlled { get; set; }
        public Location Location { get; set; }
        public DateTime DepartureTimePlanned { get; set; }
        public DateTime DepartureTimeEstimated { get; set; }
        public Transportation Transportation { get; set; }
    }

    public class Location
    {
        public string DisassembledName { get; set; }
    }

    public class Transportation
    {
        public string Name { get; set; }
        public string DisassembledName { get; set; }
        public Product Product { get; set; }
        public Destination Destination { get; set; }
        public Origin Origin { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public int IconId { get; set; }
    }

    public class Destination
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Origin
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}