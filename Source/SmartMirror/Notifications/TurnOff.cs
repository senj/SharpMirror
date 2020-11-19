using SmartMirror.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartMirror.Notifications
{
    public class TurnOff
    {
        public TurnOff(IDictionary<string, object> entities)
        {
            entities.TryGetValueAsStringListArray("HomeAutomation.DeviceType", out string[][] deviceType);
            entities.TryGetValueAsStringArray("HomeAutomation.Location", out string[] location);
            entities.TryGetValueAsStringArray("HomeAutomation.Setting", out string[] setting);

            bool isLight = deviceType.FirstOrDefault()?.Any(s => s.Equals("licht", StringComparison.OrdinalIgnoreCase)) == true;
            bool isKitchen = location.Any(s => s.Equals("küche", StringComparison.OrdinalIgnoreCase));
            if (isLight && isKitchen)
            {
                LightId = 1;
            }
        }

        public string DeviceType { get; }

        public string Location { get; }

        public string Setting { get; }

        public int LightId { get; }
    }
}

