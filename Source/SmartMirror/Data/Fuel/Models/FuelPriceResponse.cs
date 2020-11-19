
using System.Collections.Generic;

public class FuelPriceResponse
{
    public bool Ok { get; set; }
    public string License { get; set; }
    public string Data { get; set; }
    public Prices Prices { get; set; }
}

public class Prices : Dictionary<string, PriceStatus>
{
}

public class PriceStatus
{
    public string Status { get; set; }
    public float E5 { get; set; }
    public float E10 { get; set; }
    public float Diesel { get; set; }
}
