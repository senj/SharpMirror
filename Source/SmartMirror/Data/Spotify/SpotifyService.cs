using System;

namespace SmartMirror.Data.Spotify
{
    public class SpotifyService
    {
        public event EventHandler<NextSongEventArgs> NextSongRequested;

        public void PlayNextSong()
        {
            NextSongRequested?.Invoke(this, new NextSongEventArgs());
        }
    }
}
