using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMirror.Data.Jokes
{
    public class JokesQuotesService
    {
        private readonly ILogger<JokesQuotesService> _logger;
        private readonly HttpClient _httpClient;

        public JokesQuotesService(ILogger<JokesQuotesService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<ChuckNorrisJokesModel> GetNextChuckNorrisJokeAsync()
        {
            string baseUrl = "https://api.chucknorris.io/jokes/random";
            HttpResponseMessage response = await _httpClient.GetAsync(baseUrl);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting joke from chuck norris api: {statusCode}", response.StatusCode);
                return new ChuckNorrisJokesModel();
            }

            string stringResponse = await response.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
            ChuckNorrisJokesModel chuckNorrisResponse = JsonSerializer.Deserialize<ChuckNorrisJokesModel>(stringResponse, options);

            _logger.LogInformation("Got joke from chuck norris api");
            return chuckNorrisResponse;
        }
    }
}
