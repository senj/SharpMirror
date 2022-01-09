using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Fitbit
{
    [Route("fitbit")]
    [ApiController]
    public class FitbitAuthorizationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly FitbitConfiguration _fitbitConfiguration;

        public FitbitAuthorizationController(
            HttpClient httpClient,
            IOptions<FitbitConfiguration> fitbitConfiguration,
            SignInManager<IdentityUser> signInManager)
        {
            _httpClient = httpClient;
            _signInManager = signInManager;
            _fitbitConfiguration = fitbitConfiguration.Value;
        }

        [HttpGet("auth")]
        public IActionResult Auth()
        {
            string baseUrl = "https://www.fitbit.com/oauth2/authorize" +
                $"?client_id={_fitbitConfiguration.ClientId}" +
                "&response_type=code" +
                $"&redirect_uri={_fitbitConfiguration.RedirectUri}" +
                "&scope=activity%20heartrate%20location%20nutrition%20profile%20settings%20sleep%20social%20weight" +
                $"&expires_in={604800}";

            return Redirect(baseUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, [FromQuery] string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return new StatusCodeResult(500);
            }

            List<KeyValuePair<string, string>> content = new()
            {
                KeyValuePair.Create("grant_type", "authorization_code"),
                KeyValuePair.Create("code", code),
                KeyValuePair.Create("redirect_uri", _fitbitConfiguration.RedirectUri),
                KeyValuePair.Create("client_id", _fitbitConfiguration.ClientId),
                KeyValuePair.Create("client_secret", _fitbitConfiguration.ClientSecret)
            };

            FormUrlEncodedContent formContent = new(content);

            using HttpRequestMessage request = new(HttpMethod.Post, "https://api.fitbit.com/oauth2/token");
            request.Headers.Add("Authorization", "Basic " + Base64Encode($"{_fitbitConfiguration.ClientId}:{_fitbitConfiguration.ClientSecret}"));
            request.Content = formContent;
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                return new StatusCodeResult(500);
            }

            string stringResponse = await response.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
            FitbitAuthResponse authResponse = JsonSerializer.Deserialize<FitbitAuthResponse>(stringResponse, options);

            IdentityUser user = new("fitbit");
            await _signInManager.SignInAsync(user, true, "fitbit");

            return View("FitbitCallback", authResponse);
        }

        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
