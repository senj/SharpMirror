using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace SmartMirror.Data.TempSensor
{
    public class TempState : StateBase
    {
        public event Action OnDataChange;
        private readonly ILogger<TempState> _logger;
        private readonly DHTSensor _dht;

        public DHTData DHTData { get; private set; }

        public TempState(ILogger<TempState> logger, IConfiguration configuration) : base("Temperature", typeof(Shared.TempSensor))
        {
            _logger = logger;

            int gpioPin = configuration.GetValue<int>("TempSensor:GpioPinNumber");
            if (gpioPin == 0)
            {
                _logger.LogError("Unable to get GPIO number");
            }
            else
            {
                _logger.LogInformation("Using GPIO {pinNumber}", gpioPin);
            }

            try
            {
                Pi.Init<BootstrapWiringPi>();
                _dht = new(Pi.Gpio[gpioPin], DHTSensorTypes.DHT22);
            }
            catch (DllNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unable to init PI, probably not running on PI?");
            }
        }

        public async Task StartSensor()
        {
            if (_dht is null) return;

            try
            {
                while (true)
                {
                    try
                    {
                        DHTData = _dht.ReadData();
                        DHTData.DateTime = DateTime.UtcNow;
                        OnDataChange?.Invoke();
                    }
                    catch (DHTException)
                    {
                    }

                    await Task.Delay(10_000);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting temperature.");
            }
        }
    }
}
