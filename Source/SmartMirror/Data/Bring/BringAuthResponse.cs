namespace SmartMirror.Data.Bring
{
    public class BringAuthResponse
    {
        public string uuid { get; set; }
        public string publicUuid { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string photoPath { get; set; }
        public string bringListUUID { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}