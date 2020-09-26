using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Spotify
{
    [Route("spotify")]
    [ApiController]
    public class SpotifyController : Controller
    {
        private readonly ILogger<SpotifyController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SpotifyConfiguration _spotifyConfiguration;

        public event EventHandler<NextSongEventArgs> NextSongRequested;

        public SpotifyController(
            ILogger<SpotifyController> logger,
            HttpClient httpClient,
            IDistributedCache cache,
            IOptions<SpotifyConfiguration> spotifyConfiguration,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
            _signInManager = signInManager;
            _spotifyConfiguration = spotifyConfiguration.Value;
        }

        [HttpGet("auth")]
        public IActionResult Auth()
        {
            string state = Guid.NewGuid().ToString("N");
            string baseUrl = "https://accounts.spotify.com/authorize" +
                $"?client_id={_spotifyConfiguration.ClientId}" +
                "&response_type=code" +
                $"&redirect_uri={_spotifyConfiguration.RedirectUri}" +
                "&scope=user-read-private%20user-read-email%20streaming%20user-modify-playback-state" +
                $"&state={state}";

            return Redirect(baseUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, [FromQuery] string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError("Error during spotify authorization: {error}", error);   
                return new StatusCodeResult(500);
            }

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("grant_type", "authorization_code"),
                KeyValuePair.Create("code", code),
                KeyValuePair.Create("redirect_uri", _spotifyConfiguration.RedirectUri),
                KeyValuePair.Create("client_id", _spotifyConfiguration.ClientId),
                KeyValuePair.Create("client_secret", _spotifyConfiguration.ClientSecret)
            };

            FormUrlEncodedContent formContent = new FormUrlEncodedContent(content);
            HttpResponseMessage response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", formContent);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error during spotify authorization: {error} {message}", response.StatusCode, await response.Content.ReadAsStringAsync());
                return new StatusCodeResult(500);
            }

            string stringResponse = await response.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            SpotifyAuthResponse authResponse = JsonSerializer.Deserialize<SpotifyAuthResponse>(stringResponse, options);

            IdentityUser user = new IdentityUser("spotify");
            await _signInManager.SignInAsync(user, true, "spotify");

            _cache.SetString("spotify_access_token", authResponse.access_token, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(authResponse.expires_in - 30)
            });

            _cache.SetString("spotify_refresh_token", authResponse.refresh_token);

            return View("SpotifyCallback", authResponse);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            string accessToken = _cache.GetString("spotify_access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await RefreshToken(_cache.GetString("spotify_refresh_token"));
            }

            return accessToken;
        }

        public void PlayNextSong()
        {
            NextSongRequested?.Invoke(this, new NextSongEventArgs());
        }

        private async Task<string> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return string.Empty;
            }

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            };

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.fitbit.com/oauth2/token")
            {
                Content = new FormUrlEncodedContent(content)
            };

            request.Headers.Add("Authorization", EncodeHeader(_spotifyConfiguration.ClientId, _spotifyConfiguration.ClientSecret));

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string stringResponse = await response.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            SpotifyAuthResponse authResponse = JsonSerializer.Deserialize<SpotifyAuthResponse>(stringResponse, options);

            _cache.SetString("spotify_access_token", authResponse.access_token, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(authResponse.expires_in - 30)
            });

            return authResponse.access_token;
        }

        private string EncodeHeader(string clientId, string clientSecret)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
        }
    }
}
