namespace SmartMirror.Data.Fuel
{
    public class FuelStationResponse
    {
        public bool Ok { get; set; }
        public string License { get; set; }
        public string Data { get; set; }
        public string Status { get; set; }
        public Station[] Stations { get; set; }
    }
}