using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.Spotify
{
    public class SpotifyState : StateBase
    {
        public SpotifyState() : base("Spotify", typeof(Shared.Spotify))
        {
        }

        public event Func<Task> OnNextSongRequested;

        public async Task PlayNextSongAsync()
        {
            await OnNextSongRequested.Invoke();
            RaiseOnChangeEvent();
        }
    }
}
