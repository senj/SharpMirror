using System.Threading.Tasks;

namespace SmartMirror.Data.WeatherForecast
{
    public class WeatherState : Displayable
    {
        private readonly WeatherForecastService _weatherForecastService;

        public WeatherState(WeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        public OneCallWeatherForecast WeatherForecast { get; private set; }

        public async Task<OneCallWeatherForecast> GetWeatherForecastAsync()
        {
            OneCallWeatherForecast forecast = await _weatherForecastService.GetOneCallForecastAsync();
            WeatherForecast = forecast;
            RaiseOnChangeEvent();

            return forecast;
        }
    }
}
