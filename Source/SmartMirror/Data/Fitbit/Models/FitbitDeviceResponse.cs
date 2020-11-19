using System;
using System.Collections.Generic;

namespace SmartMirror.Data.Fitbit
{
    public class FitbitDeviceResponse : List<FitbitDevice>
    {
    }

    public class FitbitDevice
    {
        public string battery { get; set; }
        public int batteryLevel { get; set; }
        public string deviceVersion { get; set; }
        public object[] features { get; set; }
        public string id { get; set; }
        public DateTime lastSyncTime { get; set; }
        public string mac { get; set; }
        public string type { get; set; }
    }
}