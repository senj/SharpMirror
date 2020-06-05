using System.Text.Json.Serialization;

namespace SmartMirror.Data.WeatherForecast
{
public class Rain
{
    [JsonPropertyName("_1h")]
    public float OneHour { get; set; }
}
}