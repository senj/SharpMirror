namespace SmartMirror.Notifications
{
    public class WeatherDisplayType
    {
        public WeatherDisplayType(bool displayForecast)
        {
            DisplayForecast = displayForecast;
        }

        public bool DisplayForecast { get; }
    }
}
