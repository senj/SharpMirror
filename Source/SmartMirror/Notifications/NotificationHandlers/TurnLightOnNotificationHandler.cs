using MediatR;
using SmartMirror.SmartHome.Hue;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class TurnLightOnNotificationHandler : INotificationHandler<TurnOn>
    {
        private readonly HueService _hueService;

        public TurnLightOnNotificationHandler(HueService hueService)
        {
            _hueService = hueService;
        }

        public async Task Handle(TurnOn notification, CancellationToken cancellationToken)
        {
            LightState lightState = new LightState
            {
                on = true,
                sat = 0,
                bri = 127
            };

            await _hueService.SetLightStateAsync(notification.LightId, lightState);
        }
    }
}
