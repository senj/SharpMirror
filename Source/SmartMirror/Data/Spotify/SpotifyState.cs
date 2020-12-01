using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.Spotify
{
    public class SpotifyState : Displayable
    {
        public event Func<Task> OnNextSongRequested;

        public async Task PlayNextSongAsync()
        {
            await OnNextSongRequested.Invoke();
            RaiseOnChangeEvent();
        }
    }
}
