using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.WeatherForecast
{
    public class WeatherState
    {
        private readonly WeatherForecastService _weatherForecastService;

        public WeatherState(WeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        public event Action OnChange;

        public OneCallWeatherForecast WeatherForecast { get; private set; }

        public bool ShowDetails { get; private set; }

        public async Task<OneCallWeatherForecast> GetWeatherForecastAsync()
        {
            OneCallWeatherForecast forecast = await _weatherForecastService.GetOneCallForecastAsync();
            WeatherForecast = forecast;
            OnChange?.Invoke();

            return forecast;
        }

        public void SetShowDetails(bool showDetails)
        {
            ShowDetails = showDetails;
            OnChange?.Invoke();
        }
    }
}
