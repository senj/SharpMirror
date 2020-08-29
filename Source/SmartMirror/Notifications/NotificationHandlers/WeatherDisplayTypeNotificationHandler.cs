using MediatR;
using SmartMirror.Data.WeatherForecast;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class WeatherDisplayTypeNotificationHandler : INotificationHandler<WeatherDisplayType>
    {
        private readonly WeatherForecastService _weatherForecastService;

        public WeatherDisplayTypeNotificationHandler(WeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        public Task Handle(WeatherDisplayType request, CancellationToken cancellationToken)
        {
            _weatherForecastService.WeatherDisplayType(request.DisplayForecast);
            return Task.CompletedTask;
        }
    }
}
