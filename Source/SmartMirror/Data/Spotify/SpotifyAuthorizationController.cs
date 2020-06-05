using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Spotify
{
    [Route("spotify")]
    [ApiController]
    public class SpotifyAuthorizationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SpotifyConfiguration _spotifyConfiguration;

        public SpotifyAuthorizationController(
            HttpClient httpClient, 
            IOptions<SpotifyConfiguration> spotifyConfiguration,
            SignInManager<IdentityUser> signInManager)
        {
            _httpClient = httpClient;
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

            var formContent = new FormUrlEncodedContent(content);
            var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", formContent);
            
            if (!response.IsSuccessStatusCode)
            {
                return new StatusCodeResult(500);
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var authResponse = JsonSerializer.Deserialize<SpotifyAuthResponse>(stringResponse, options);

            IdentityUser user = new IdentityUser("spotify");
            await _signInManager.SignInAsync(user, true, "spotify");

            return View("SpotifyCallback", authResponse);
        }
    }
}
