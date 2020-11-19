namespace SmartMirror.Data.GoogleFit
{
    public class GoogleCodeResponse
    {
        public string device_code { get; set; }
        public string user_code { get; set; }
        public string verification_url { get; set; }
        public int expires_in { get; set; }
        public int interval { get; set; }
        public string error_code { get; set; }
    }
}
