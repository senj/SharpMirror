using MediatR;
using SmartMirror.Data.Spotify;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMirror.Notifications.NotificationHandlers
{
    public class NextSongRequestedNotificationHandler : INotificationHandler<NextSongRequested>
    {
        private readonly SpotifyService _spotifyService;

        public NextSongRequestedNotificationHandler(SpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        public Task Handle(NextSongRequested notification, CancellationToken cancellationToken)
        {
            _spotifyService.PlayNextSong();
            return Task.CompletedTask;
        }
    }
}
