namespace SmartMirror.Data.WeatherForecast
{
public class Hourly
{
    public int Dt { get; set; }
    public float Temp { get; set; }
    public float Feels_like { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public float Dew_point { get; set; }
    public int Clouds { get; set; }
    public float Wind_speed { get; set; }
    public int Wind_deg { get; set; }
    public Weather[] Weather { get; set; }
    public Rain Rain { get; set; }
}
}