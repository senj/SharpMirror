using MediatR;
using SmartMirror.Data.WeatherForecast;

namespace SmartMirror.Notifications
{
    public class WeatherInformationRequest : IRequest<OneCallWeatherForecast>
    {
        public WeatherInformationRequest(bool displayForecast)
        {
            DisplayForecast = displayForecast;
        }

        public bool DisplayForecast { get; }
    }
}
