using System;
using System.Threading.Tasks;

namespace SmartMirror.SmartHome.Hue
{
    public class HueState
    {
        private readonly HueService _hueService;

        public HueState(HueService hueService)
        {
            _hueService = hueService;
        }

        public event Action OnChange;

        public bool ShowDetails { get; private set; }

        public HueLightInfo HueLightInfo { get; private set; }

        public async Task<HueLightInfo> GetLightInfoAsync(int lightId)
        {
            HueLightInfo = await _hueService.GetLightInfoAsync(lightId);
            OnChange?.Invoke();

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

        public void SetShowDetails(bool showDetails)
        {
            ShowDetails = showDetails;
            OnChange?.Invoke();
        }
    }
}
