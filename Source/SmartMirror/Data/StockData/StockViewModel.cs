using System.Collections.Generic;
using System.Text.Json;

namespace SmartMirror.Data.StockData
{
    public class StockViewModel
    {
        public IEnumerable<JsonProperty> StockData { get; set; }
    }
}
