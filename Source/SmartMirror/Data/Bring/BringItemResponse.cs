namespace SmartMirror.Data.Bring
{
    public class BringItemResponse
    {
        public string uuid { get; set; }
        public string status { get; set; }
        public Purchase[] purchase { get; set; }
        public Recently[] recently { get; set; }
    }

    public class Purchase
    {
        public string specification { get; set; }
        public string name { get; set; }
    }

    public class Recently
    {
        public string specification { get; set; }
        public string name { get; set; }
    }
}