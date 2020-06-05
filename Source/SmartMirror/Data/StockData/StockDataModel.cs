using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartMirror.Data.StockData
{
    public class StockDataModel
    {
        public MetaData MetaData { get; set; }

        [JsonPropertyName("TimeSeries(5min)")]
        public JsonElement? TimeSeries { get; set; }
    }

    public class MetaData
    {
        [JsonPropertyName("1.Information")]
        public string Information { get; set; }

        [JsonPropertyName("2.Symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("3.LastRefreshed")]
        public string LastRefreshed { get; set; }

        [JsonPropertyName("4.Interval")]
        public string Interval { get; set; }

        [JsonPropertyName("5.OutputSize")]
        public string OutputSize { get; set; }

        [JsonPropertyName("6.TimeZone")]
        public string TimeZone { get; set; }
    }

    public class DataSet
    {
        [JsonPropertyName("1.open")]
        public double Open { get; set; }

        [JsonPropertyName("2.high")]
        public double High { get; set; }

        [JsonPropertyName("3.low")]
        public double Low { get; set; }

        [JsonPropertyName("4.close")]
        public double Close { get; set; }

        [JsonPropertyName("5.volume")]
        public double Volume { get; set; }
    }
}