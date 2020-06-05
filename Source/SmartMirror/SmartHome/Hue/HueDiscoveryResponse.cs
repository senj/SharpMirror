using System.Collections.Generic;

namespace SmartMirror.SmartHome.Hue
{
    public class HueDiscoveryResponse : List<Tool>
    {
    }

    public class Tool
    {
        public string id { get; set; }
        public string internalipaddress { get; set; }
    }
}