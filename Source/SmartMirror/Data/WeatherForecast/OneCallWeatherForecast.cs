namespace SmartMirror.Data.WeatherForecast
{
    public class OneCallWeatherForecast
    {
        public float Lat { get; set; }

        public float Lon { get; set; }

        public string Timezone { get; set; }

        public Current Current { get; set; }

        public Hourly[] Hourly { get; set; }

        public Daily[] Daily { get; set; }
    }
}
