﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartMirror.Data.GoogleFit
{
    public class GoogleFitService
    {
        private const string _cacheKeyAccessToken = "google_access_token";
        private const string _cacheKeyRefreshToken = "google_refresh_token";

        private readonly ILogger<GoogleFitService> _logger;
        private readonly IDistributedCache _cache;
        private readonly HttpClient _httpClient;
        private readonly GoogleApiConfiguration _configuration;

        public GoogleFitService(
            ILogger<GoogleFitService> logger,
            IOptions<GoogleApiConfiguration> configuration,
            IDistributedCache cache,
            HttpClient httpClient)
        {
            _logger = logger;
            _cache = cache;
            _httpClient = httpClient;
            _configuration = configuration.Value;
        }

        private string DeviceRegistrationEndpoint { get; set; }

        public async Task<GoogleCodeResponse> StartAuthorizationAsync()
        {
            List<KeyValuePair<string, string>> content = new()
            {
                new KeyValuePair<string, string>("client_id", _configuration.ClientId),
                new KeyValuePair<string, string>("scope", "openid profile https://www.googleapis.com/auth/fitness.body.read https://www.googleapis.com/auth/fitness.activity.read")
            };

            DeviceRegistrationEndpoint = await GetDeviceRegistrationEndpointAsync();
            HttpResponseMessage response = await _httpClient.PostAsync(DeviceRegistrationEndpoint, new FormUrlEncodedContent(content));

            string responseContent = await response.Content.ReadAsStringAsync();
            GoogleCodeResponse codeResponse = JsonConvert.DeserializeObject<GoogleCodeResponse>(responseContent);

            return codeResponse;
        }

        public async Task<GoogleAuthResponse> AuthorizationPolling(string user, GoogleCodeResponse googleCodeResponse)
        {
            DateTime expiresDateTime = DateTime.UtcNow.AddSeconds(googleCodeResponse.expires_in);
            List<KeyValuePair<string, string>> content = new()
            {
                new KeyValuePair<string, string>("client_id", _configuration.ClientId),
                new KeyValuePair<string, string>("client_secret", _configuration.ClientSecret),
                new KeyValuePair<string, string>("device_code", googleCodeResponse.device_code),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code")
            };

            while (DateTime.UtcNow < expiresDateTime && string.IsNullOrEmpty(await GetAccessTokenAsync(user)))
            {
                HttpResponseMessage response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(content));

                string responseContent = await response.Content.ReadAsStringAsync();
                GoogleAuthResponse googleAuthResponse = JsonConvert.DeserializeObject<GoogleAuthResponse>(responseContent);

                if (response.IsSuccessStatusCode)
                {
                    _cache.SetString(GetCacheKey(_cacheKeyRefreshToken, user), googleAuthResponse.refresh_token);
                    _cache.SetString(GetCacheKey(_cacheKeyAccessToken, user), googleAuthResponse.access_token, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(googleAuthResponse.expires_in - 30)
                    });
                    
                    return googleAuthResponse;
                }

                _logger.LogWarning("Error polling google auth endpoint: {statusCode}, {error}", response.StatusCode, googleAuthResponse.error);
                await Task.Delay(googleCodeResponse.interval * 1000);
            }

            return new GoogleAuthResponse
            {
                access_token = await GetAccessTokenAsync(user)
            };
        }

        public async Task<IEnumerable<WeightDataPoint>> GetWeight(string accessToken, int take)
        {
            string dataSourceId = "derived:com.google.weight:com.google.android.gms:merge_weight";
            HttpRequestMessage request = new(HttpMethod.Get, $"https://www.googleapis.com/fitness/v1/users/me/dataSources/{dataSourceId}/dataPointChanges");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            GoogleDerivedWeightResponse weightResponse = JsonConvert.DeserializeObject<GoogleDerivedWeightResponse>(await response.Content.ReadAsStringAsync());
            IEnumerable<WeightDataPoint> weightValues = weightResponse?.insertedDataPoint?
                .Select(p => new WeightDataPoint(p.startTimeNanos, p.endTimeNanos, p.value.FirstOrDefault()?.fpVal));

            if (weightValues == null)
            {
                return Enumerable.Empty<WeightDataPoint>();
            }

            return weightValues
                .OrderBy(p => p.StartTime)
                .Take(take);
        }

        public async Task<string> GetAccessTokenAsync(string user)
        {
            string accessToken = _cache.GetString(GetCacheKey(_cacheKeyAccessToken, user));
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await RefreshToken(_cache.GetString(GetCacheKey(_cacheKeyRefreshToken, user)), user);
            }

            return accessToken;
        }

        private async Task<string> RefreshToken(string refreshToken, string user)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return string.Empty;
            }

            List<KeyValuePair<string, string>> content = new()
            {
                new KeyValuePair<string, string>("client_id", _configuration.ClientId),
                new KeyValuePair<string, string>("client_secret", _configuration.ClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            };

            HttpResponseMessage response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(content));
            GoogleRefreshTokenResponse refreshTokenResponse = JsonConvert.DeserializeObject<GoogleRefreshTokenResponse>(await response.Content.ReadAsStringAsync());
            _cache.SetString(GetCacheKey(_cacheKeyAccessToken, user), refreshTokenResponse.access_token, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(refreshTokenResponse.expires_in - 30)
            });

            return refreshTokenResponse.access_token;
        }

        private async Task<string> GetDeviceRegistrationEndpointAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("https://accounts.google.com/.well-known/openid-configuration");
            GoogleDiscoveryResponse discoveryResponse = JsonConvert.DeserializeObject<GoogleDiscoveryResponse>(await response.Content.ReadAsStringAsync());
            return discoveryResponse.device_authorization_endpoint;
        }

        private static string GetCacheKey(string purpose, string user)
        {
            return $"{purpose}_{user}";
        }
    }
}
