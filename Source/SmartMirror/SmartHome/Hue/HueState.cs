using SmartMirror.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.SmartHome.Hue
{
    public class HueState : StateBase
    {
        private readonly HueService _hueService;

        public HueState(HueService hueService) : base("Hue", typeof(Shared.HueLights))
        {
            _hueService = hueService;
            HueLightInfo = new Dictionary<int, HueLightInfo>();
        }

        public Dictionary<int, HueLightInfo> HueLightInfo { get; private set; }

        public async Task<HueLightInfo> GetLightInfoAsync(int lightId)
        {
            var hueLightInfo = await _hueService.GetLightInfoAsync(lightId);
            if (HueLightInfo.ContainsKey(lightId))
            {
                HueLightInfo[lightId] = hueLightInfo;
            }
            else
            {
                HueLightInfo.Add(lightId, hueLightInfo);
            }

            RaiseOnChangeEvent();
            return hueLightInfo;
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
