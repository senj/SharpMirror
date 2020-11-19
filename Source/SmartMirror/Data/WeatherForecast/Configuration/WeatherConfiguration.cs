namespace SmartMirror.Data.WeatherForecast
{
    public class WeatherConfiguration
    {
        public string ApiKey { get; set; }

        public string Language { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public string Unit { get; set; }
    }
}
