using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.SmartHome.Hue
{
    public class HueService
    {
        private readonly ILogger<HueService> _logger;
        private readonly string _ipAddress;
        private readonly HttpClient _httpClient;
        private readonly string _userToken = "T165psyqi-UxsIcRQe0TIK5cu2MinJbJtZsVDKKD";

        public event EventHandler<LightStateChangedEventArgs> LightStateChanged;

        public HueService(ILogger<HueService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _ipAddress = "192.168.0.81"; 
            // GetIpAddressAsync().GetAwaiter().GetResult();
            _logger.LogInformation("Got IP Address for Hue: {ipAddress}", _ipAddress);
        }

        public async Task<bool> SetLightStateAsync(int lightId, LightState lightState)
        {
            _logger.LogInformation("Turning light {lightId} on: {isOn}", lightId, lightState.on);

            HttpResponseMessage response = await _httpClient.PutAsync($"http://{_ipAddress}/api/{_userToken}/lights/{lightId}/state",
                new StringContent(JsonSerializer.Serialize(lightState), Encoding.UTF8, "application/json"));

            LightStateChanged?.Invoke(this, new LightStateChangedEventArgs());

            return response.IsSuccessStatusCode;
        }

        public async Task<HueLightInfo> GetLightInfoAsync(int lightId)
        {
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync($"http://{_ipAddress}/api/{_userToken}/lights/{lightId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to load light state form address {address}.", _ipAddress);
                return null;
            }

            string stringResponse = await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
            HueLightInfo lightInfo = JsonSerializer.Deserialize<HueLightInfo>(stringResponse, options);

            return lightInfo;
        }

        public async Task<string> GetIpAddressAsync()
        {
            using HttpClient httpClient = new();
            HttpResponseMessage response = await httpClient.GetAsync("https://discovery.meethue.com/");
            string stringResponse = await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
            HueDiscoveryResponse discoveryResponse = JsonSerializer.Deserialize<HueDiscoveryResponse>(stringResponse, options);

            string ipAddress = discoveryResponse.FirstOrDefault()?.internalipaddress;

            return ipAddress;
        }
    }

    public class LightStateChangedEventArgs
    {
    }
}
