namespace SmartMirror.Notifications
{
    public class WeatherInformationRequest
    {
        public WeatherInformationRequest(bool displayForecast)
        {
            DisplayForecast = displayForecast;
        }

        public bool DisplayForecast { get; }
    }
}
