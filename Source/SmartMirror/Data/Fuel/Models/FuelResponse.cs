using System;
using System.Collections.Generic;

namespace SmartMirror.Data.Fuel
{
    public class FuelResponse : Dictionary<string, List<FuelResult>>
    {
    }

    public class FuelResult
    {
        public FuelResult()
        {
        }

        public FuelResult(DateTimeOffset timestamp, Station station, PriceStatus priceStatus)
        {
            TimeStamp = timestamp;
            Station = station;
            PriceStatus = priceStatus;
        }

        public DateTimeOffset TimeStamp { get; set; }

        public Station Station { get; set; }
        
        public PriceStatus PriceStatus { get; set; }
    }
}
