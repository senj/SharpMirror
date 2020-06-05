using System;

namespace SmartMirror.Data.VVS
{
    public class VvsResponse
    {
        public string Version { get; set; }
        public object[] SystemMessages { get; set; }
        public Location[] Locations { get; set; }
        public Stopevent[] StopEvents { get; set; }
    }

    public class Location
    {
        public string Id { get; set; }
        public bool IsGlobalId { get; set; }
        public string Name { get; set; }
        public string DisassembledName { get; set; }
        public float[] Coord { get; set; }
        public string Type { get; set; }
        public int MatchQuality { get; set; }
        public bool IsBest { get; set; }
        public Parent Parent { get; set; }
        public Assignedstop[] AssignedStops { get; set; }
        public Properties Properties { get; set; }
        public Download[] Downloads { get; set; }
    }

    public class Parent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DisassembledName { get; set; }
    }

    public class Properties
    {
        public string StopId { get; set; }
    }

    public class Assignedstop
    {
        public string Id { get; set; }
        public bool IsGlobalId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public float[] Coord { get; set; }
        public Parent Parent { get; set; }
        public int[] ProductClasses { get; set; }
        public int ConnectingMode { get; set; }
        public Properties Properties { get; set; }
    }

    public class Download
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public int Size { get; set; }
    }

    public class Stopevent
    {
        public bool IsRealtimeControlled { get; set; }
        public Location Location { get; set; }
        public DateTime DepartureTimePlanned { get; set; }
        public DateTime DepartureTimeEstimated { get; set; }
        public Transportation Transportation { get; set; }
        public Properties Properties { get; set; }
        public Info[] Infos { get; set; }
        public Hint[] Hints { get; set; }
    }

    public class Transportation
    {
        public string id { get; set; }
        public string name { get; set; }
        public string disassembledName { get; set; }
        public string number { get; set; }
        public string description { get; set; }
        public Product product { get; set; }
        public Operator _operator { get; set; }
        public Destination destination { get; set; }
        public Properties2 properties { get; set; }
        public Origin origin { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public int _class { get; set; }
        public string name { get; set; }
        public int iconId { get; set; }
    }

    public class Operator
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Destination
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Properties2
    {
        public string trainName { get; set; }
        public string trainNumber { get; set; }
        public bool isROP { get; set; }
        public int tripCode { get; set; }
        public string mtSubcode { get; set; }
        public bool isTTB { get; set; }
        public bool isSTT { get; set; }
    }

    public class Origin
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Properties3
    {
        public string AVMSTripID { get; set; }
        public string platformChange { get; set; }
    }

    public class Info
    {
        public string priority { get; set; }
        public string id { get; set; }
        public string version { get; set; }
        public string urlText { get; set; }
        public string url { get; set; }
        public string content { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public string additionalText { get; set; }
        public Properties4 properties { get; set; }
    }

    public class Properties4
    {
        public string publisher { get; set; }
        public string infoType { get; set; }
        public string timetableChange { get; set; }
        public string htmlText { get; set; }
        public string smsText { get; set; }
    }

    public class Hint
    {
        public string content { get; set; }
    }
}