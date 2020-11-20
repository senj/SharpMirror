namespace SmartMirror.Intents
{
    public class WeatherDisplayForecast
    {
        public WeatherDisplayForecast(bool displayForecast)
        {
            DisplayForecast = displayForecast;
        }

        public bool DisplayForecast { get; }
    }
}
