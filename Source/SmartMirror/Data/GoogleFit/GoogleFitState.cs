using SmartMirror.Data.GoogleFit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.GoogleFit
{
    public class GoogleFitState : UserState
    {
        private readonly GoogleFitService _googleFitService;
        
        public GoogleFitState(GoogleFitService googleFitService) : base("GoogleFit", typeof(Shared.GoogleFit))
        {
            _googleFitService = googleFitService;

            InitNewUserState<IEnumerable<WeightDataPoint>>();
            InitNewUserState<GoogleCodeResponse>();
            InitNewUserState<GoogleAccessToken>();
        }

        public IEnumerable<WeightDataPoint> GetWeightData(string user)
        {
            return GetUserState<IEnumerable<WeightDataPoint>>(user);
        }

        public GoogleCodeResponse GetGoogleCodeResponse(string user)
        {
            return GetUserState<GoogleCodeResponse>(user);
        }

        public string GetToken(string user)
        {
            return GetUserState<GoogleAccessToken>(user)?.Token;
        }

        public async Task StartAuthorizationAsync(string user)
        {
            if (Enabled)
            {
                GoogleCodeResponse googleCodeResponse = await _googleFitService.StartAuthorizationAsync();
                SetUserState<GoogleCodeResponse>(user, googleCodeResponse);
                RaiseOnChangeEvent();
            }
        }

        public async Task AuthorizationPollingAsync(string user)
        {
            if (Enabled)
            {
                GoogleCodeResponse googleCodeResponse = GetGoogleCodeResponse(user);
                GoogleAuthResponse googleAuthResponse = await _googleFitService.AuthorizationPolling(user, googleCodeResponse);
                string token = googleAuthResponse?.access_token;
                SetUserState<GoogleAccessToken>(user, new GoogleAccessToken(token));
                RaiseOnChangeEvent();
            }
        }

        public async Task UpdateWeightData(string user, int take)
        {
            string token = GetToken(user);
            IEnumerable<WeightDataPoint> weightData = await _googleFitService.GetWeight(token, take);
            SetUserState<IEnumerable<WeightDataPoint>>(user, weightData);
            RaiseOnChangeEvent();
        }
    }
}
