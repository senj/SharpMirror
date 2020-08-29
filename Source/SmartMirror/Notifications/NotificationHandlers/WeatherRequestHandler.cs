using MediatR;
using SmartMirror.Data.WeatherForecast;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class WeatherRequestHandler : IRequestHandler<WeatherInformationRequest, OneCallWeatherForecast>
    {
        private readonly WeatherForecastService _weatherForecastService;

        public WeatherRequestHandler(WeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        public Task<OneCallWeatherForecast> Handle(WeatherInformationRequest request, CancellationToken cancellationToken)
        {
            return _weatherForecastService.GetOneCallForecastAsync(request.DisplayForecast);
        }
    }
}
