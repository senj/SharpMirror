using System;

namespace SmartMirror.Data.Calendar
{
    public class Event
    {
        public DateTime DtStart { get; set; }
        
        public DateTime DtEnd { get; set; }

        public bool IsAllDay { get; set; }

        public string Summary { get; set; }
    }
}
