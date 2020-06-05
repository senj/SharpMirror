namespace SmartMirror.Data.WeatherForecast
{
public class Daily
{
    public int Dt { get; set; }
    public int Sunrise { get; set; }
    public int Sunset { get; set; }
    public Temp Temp { get; set; }
    public FeelsLike Feels_like { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public float Dew_point { get; set; }
    public float Wind_speed { get; set; }
    public int Wind_deg { get; set; }
    public Weather[] Weather { get; set; }
    public int Clouds { get; set; }
    public float Uvi { get; set; }
    public float Rain { get; set; }
}
}