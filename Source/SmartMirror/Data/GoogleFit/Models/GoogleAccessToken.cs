namespace SmartMirror.Data.GoogleFit.Models
{
    public class GoogleAccessToken
    {
        public GoogleAccessToken(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
