namespace SmartMirror.Data.WeatherForecast
{
    public class ForecastEventArgs
    {
        public ForecastEventArgs(bool displayForecast)
        {
            DisplayForecast = displayForecast;
        }

        public bool DisplayForecast { get; set; }
    }
}