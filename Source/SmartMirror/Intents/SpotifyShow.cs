namespace SmartMirror.Intents
{
    public class SpotifyShow
    {
        public SpotifyShow(bool displaySpotify)
        {
            DisplaySpotify = displaySpotify;
        }

        public bool DisplaySpotify { get; }
    }
}
