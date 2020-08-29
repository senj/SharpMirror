using MediatR;

namespace SmartMirror.Notifications
{
    public class WeatherDisplayType : INotification
    {
        public WeatherDisplayType(bool displayForecast)
        {
            DisplayForecast = displayForecast;
        }

        public bool DisplayForecast { get; }
    }
}
