using System;

namespace SmartMirror.Data.GoogleFit
{
    public class WeightDataPoint
    {
        private static readonly DateTime _epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DateTime StartTime;
        public DateTime EndTime;
        public float? Value;

        public WeightDataPoint(string startTimeNanos, string endTimeNanos, float? fpVal)
        {
            StartTime = FromUnixTime(double.Parse(startTimeNanos) * 0.000001);
            EndTime = FromUnixTime(double.Parse(endTimeNanos) * 0.000001);
            Value = fpVal;
        }

        public static DateTime FromUnixTime(double unixTime)
        {
            return _epoch.AddMilliseconds(unixTime);
        }
    }
}
