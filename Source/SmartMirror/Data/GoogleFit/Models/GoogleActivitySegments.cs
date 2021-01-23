namespace SmartMirror.Data.GoogleFit.Models
{
    public class GoogleActivitySegments
    {
        public Inserteddatapoint[] insertedDataPoint { get; set; }
        public Deleteddatapoint[] deletedDataPoint { get; set; }
        public string nextPageToken { get; set; }
        public string dataSourceId { get; set; }
    }

    public class Inserteddatapoint
    {
        public string startTimeNanos { get; set; }
        public string endTimeNanos { get; set; }
        public string dataTypeName { get; set; }
        public string originDataSourceId { get; set; }
        public Value[] value { get; set; }
        public string modifiedTimeMillis { get; set; }
    }

    public class Value
    {
        public int intVal { get; set; }
        public object[] mapVal { get; set; }
    }

    public class Deleteddatapoint
    {
        public string startTimeNanos { get; set; }
        public string endTimeNanos { get; set; }
        public string dataTypeName { get; set; }
        public string originDataSourceId { get; set; }
        public Value1[] value { get; set; }
        public string modifiedTimeMillis { get; set; }
    }

    public class Value1
    {
        public int intVal { get; set; }
        public object[] mapVal { get; set; }
    }

}
