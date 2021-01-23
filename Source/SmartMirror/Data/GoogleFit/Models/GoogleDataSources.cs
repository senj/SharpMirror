namespace SmartMirror.Data.GoogleFit.Models
{
    public class GoogleDataSources
    {
        public Datasource[] dataSource { get; set; }
    }

    public class Datasource
    {
        public string dataStreamId { get; set; }
        public string dataStreamName { get; set; }
        public string type { get; set; }
        public Datatype dataType { get; set; }
        public Application application { get; set; }
        public object[] dataQualityStandard { get; set; }
        public Device device { get; set; }
        public string name { get; set; }
    }

    public class Datatype
    {
        public string name { get; set; }
        public Field[] field { get; set; }
    }

    public class Field
    {
        public string name { get; set; }
        public string format { get; set; }
        public bool optional { get; set; }
    }

    public class Application
    {
        public string packageName { get; set; }
        public string version { get; set; }
        public string name { get; set; }
    }

    public class Device
    {
        public string uid { get; set; }
        public string type { get; set; }
        public string version { get; set; }
        public string model { get; set; }
        public string manufacturer { get; set; }
    }
}
