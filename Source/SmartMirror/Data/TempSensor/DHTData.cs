using System;

namespace SmartMirror.Data.TempSensor
{
    public class DHTData
    {
        public float TempCelcius { get; set; }

        public float TempFahrenheit { get; set; }

        public float Humidity { get; set; }

        public double HeatIndex { get; set; }

        public DateTime DateTime { get; set; }
    }
}
