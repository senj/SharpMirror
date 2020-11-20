using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.Spotify
{
    public class SpotifyState
    {
        public event Action OnChange;

        public event Func<Task> OnNextSongRequested;

        public bool ShowDetails { get; private set; }

        public async Task PlayNextSongAsync()
        {
            await OnNextSongRequested.Invoke();
            OnChange.Invoke();
        }

        public void SetShowDetails(bool showDetails)
        {
            ShowDetails = showDetails;
            OnChange?.Invoke();
        }
    }
}
