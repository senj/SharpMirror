using System;

namespace SmartMirror.Data.GoogleFit
{
    public class ActivityDataPoint
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DateTime StartTime;
        public DateTime EndTime;
        public TimeSpan Duration;
        public int? Type;
        public string Name;

        public ActivityDataPoint(string startTimeNanos, string endTimeNanos, int? type)
        {
            StartTime = FromUnixTime(double.Parse(startTimeNanos) * 0.000001);
            EndTime = FromUnixTime(double.Parse(endTimeNanos) * 0.000001);
            Duration = EndTime - StartTime;
            Type = type;
            Name = GetName(type);
        }

        private string GetName(int? type)
        {
            return type switch
            {
                1 => "Biking",
                7 => "Walking",
                8 => "Running",
                9 => "Aerobics",
                100 => "Joga",
                4 => "Unknown",
                _ => "Unknown"
            };
        }

        public static DateTime FromUnixTime(double unixTime)
        {
            return _epoch.AddMilliseconds(unixTime);
        }
    }
}
