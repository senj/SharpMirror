using System;

namespace SmartMirror.Data.GoogleFit
{
    public class GoogleDerivedWeightResponse
    {
        public Inserteddatapoint[] insertedDataPoint { get; set; }
        public object[] deletedDataPoint { get; set; }
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
        public float fpVal { get; set; }
        public object[] mapVal { get; set; }
    }

}