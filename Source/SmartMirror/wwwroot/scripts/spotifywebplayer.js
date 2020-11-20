window.onSpotifyWebPlaybackSDKReady = () => {
    const player = new Spotify.Player({
        name: 'Smart Mirror',
        getOAuthToken: cb => {
            // try get token
            var accessToken = localStorage.getItem('spotify_access_token')
            if (accessToken !== null) {
                // check if token is still valid
                var expires_at = localStorage.getItem('spotify_expires_at')
                var expires = new Date(decodeHtml(expires_at));
                var currentDate = new Date(Date.now())
                if (expires < currentDate) {
                    // no longer valid, continue
                    localStorage.clear();
                }
                else {
                    // still valid, just return token
                    cb(accessToken);
                    return;
                }
            }

            // open new window to login to spotify
            popup = window.open(
                '/spotify/auth',
                'Login with Spotify',
                'width=800,height=600'
            );

            var timer = setInterval(function () {
                // try getting access token from storage
                var accessToken = localStorage.getItem('spotify_access_token')
                if (accessToken !== null) {
                    clearInterval(timer);
                    cb(accessToken);
                }
            }, 500);
        },
        volume: 0.2
    });

    // Error handling
    player.addListener('initialization_error', ({ message }) => { console.error(message); });
    player.addListener('authentication_error', ({ message }) => { console.error(message); });
    player.addListener('account_error', ({ message }) => { console.error(message); });
    player.addListener('playback_error', ({ message }) => { console.error(message); });

    // Playback status updates
    player.addListener('player_state_changed', state => {
        console.log(state);

        if (state === null) {
            return;
        }

        let {
            current_track,
            next_tracks: [next_track]
        } = state.track_window;

        document.getElementById('spotify_track').innerHTML = current_track.name;
        document.getElementById('spotify_artist').innerHTML = '';
        for (let i = 0; i < current_track.artists.length; i++) {
            document.getElementById('spotify_artist').innerHTML += current_track.artists[i].name;
            if (i < current_track.artists.length - 1) {
                document.getElementById('spotify_artist').innerHTML += ', ';
            }
        }

        if (document.getElementById('spotify_play') !== null) {
            document.getElementById('spotify_play').innerHTML = '&#9654';
            document.getElementById('spotify_image').src = current_track.album.images[0].url;

            document.getElementById('spotify_next').innerHTML = '&#9193';
            document.getElementById('spotify_next_track').innerHTML = next_track.name;
            document.getElementById('spotify_next_artist').innerHTML = '';
            for (let i = 0; i < next_track.artists.length; i++) {
                document.getElementById('spotify_next_artist').innerHTML += next_track.artists[i].name;
                if (i < next_track.artists.length - 1) {
                    document.getElementById('spotify_next_artist').innerHTML += ', ';
                }
            }
        }
    });

    // Ready
    player.addListener('ready', ({ device_id }) => {
        document.getElementById('spotify_image').src = '/images/spotify/spotify_green.png';
        console.log('Ready with Device ID', device_id);
    });

    // Not Ready
    player.addListener('not_ready', ({ device_id }) => {
        document.getElementById('spotify_image').src = '/images/spotify/spotify_red.png';
        console.log('Device ID has gone offline', device_id);
    });

    // Connect to the player!
    player.connect().then(success => {
        if (success) {
            console.log('The Web Playback SDK successfully connected to Spotify!');
        }
    })
};

function nextSong() {
    fetch('https://api.spotify.com/v1/me/player/next', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('spotify_access_token')
        },
    });
}

//const play = ({
//    "spotify:track:7xGfFoTpQ2E7fRF5lN10tr",
//    player
//})

function decodeHtml(html) {
    var txt = document.createElement("textarea");
    txt.innerHTML = html;
    return txt.value;
}