using MediatR;
using SmartMirror.SmartHome.Hue;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class TurnLightOffNotificationHandler : INotificationHandler<TurnOff>
    {
        private readonly HueService _hueService;

        public TurnLightOffNotificationHandler(HueService hueService)
        {
            _hueService = hueService;
        }

        public async Task Handle(TurnOff notification, CancellationToken cancellationToken)
        {
            LightState lightState = new LightState
            {
                on = false
            };

            await _hueService.SetLightStateAsync(notification.LightId, lightState);
        }
    }
}
