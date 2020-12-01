using SmartMirror.Data;
using System;
using System.Threading.Tasks;

namespace SmartMirror.SmartHome.Hue
{
    public class HueState : Displayable
    {
        private readonly HueService _hueService;

        public HueState(HueService hueService)
        {
            _hueService = hueService;
        }

        public HueLightInfo HueLightInfo { get; private set; }

        public async Task<HueLightInfo> GetLightInfoAsync(int lightId)
        {
            HueLightInfo = await _hueService.GetLightInfoAsync(lightId);
            RaiseOnChangeEvent();

            return HueLightInfo;
        }

        public async Task SetLightStateAsync(int lightId, LightState lightState)
        {
            bool setLightResponse = await _hueService.SetLightStateAsync(lightId, lightState);
            if (setLightResponse)
            {
                await GetLightInfoAsync(lightId);
            }
        }
    }
}
