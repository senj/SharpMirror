namespace SmartMirror.Intents
{
    public class WeatherShow
    {
        public WeatherShow(bool displayWeather)
        {
            DisplayWeather = displayWeather;
        }

        public bool DisplayWeather { get; }
    }
}
