using SmartMirror.Data.GoogleFit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.GoogleFit
{
    public class GoogleFitState : UserState
    {
        private readonly GoogleFitService _googleFitService;

        public GoogleFitState(GoogleFitService googleFitService)
        {
            _googleFitService = googleFitService;

            InitNewUserState<IEnumerable<WeightDataPoint>>();
            InitNewUserState<IEnumerable<ActivityDataPoint>>();
            InitNewUserState<GoogleCodeResponse>();
            InitNewUserState<GoogleAccessToken>();
        }

        public IEnumerable<WeightDataPoint> GetWeightData(string user)
        {
            return GetUserState<IEnumerable<WeightDataPoint>>(user);
        }

        public IEnumerable<ActivityDataPoint> GetActivityData(string user)
        {
            return GetUserState<IEnumerable<ActivityDataPoint>>(user);
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
            GoogleCodeResponse googleCodeResponse = await _googleFitService.StartAuthorizationAsync();
            SetUserState<GoogleCodeResponse>(user, googleCodeResponse);
            RaiseOnChangeEvent();
        }

        public async Task AuthorizationPollingAsync(string user)
        {
            GoogleCodeResponse googleCodeResponse = GetGoogleCodeResponse(user);
            GoogleAuthResponse googleAuthResponse = await _googleFitService.AuthorizationPolling(user, googleCodeResponse);
            string token = googleAuthResponse?.access_token;
            SetUserState<GoogleAccessToken>(user, new GoogleAccessToken(token));
            RaiseOnChangeEvent();
        }

        public async Task UpdateWeightData(string user, int take)
        {
            string token = GetToken(user);

            await _googleFitService.GetDataSources(token, take);

            IEnumerable<ActivityDataPoint> activities = await _googleFitService.GetActivities("derived:com.google.calories.expended:com.google.android.gms:from_activities", token, take);
            await _googleFitService.GetActivities("derived:com.google.activity.segment:com.google.android.gms:merge_activity_segments", token, take);
            await _googleFitService.GetActivities("derived:com.google.active_minutes:com.google.android.gms:merge_active_minutes", token, take);
            await _googleFitService.GetActivities("derived:com.google.activity.segment:com.google.android.gms:Google:Pixel 4a:e630aff8:from_activity_samples\u003c-derived:com.google.activity.samples:com.google.android.gms:Google:Pixel 4a:e630aff8:detailed", token, take);
            await _googleFitService.GetActivities("derived:com.google.active_minutes:com.google.android.gms:from_activity\u003c-merge_activity_segments", token, take);

            IEnumerable<WeightDataPoint> weightData = await _googleFitService.GetWeight(token, take);
            
            SetUserState<IEnumerable<WeightDataPoint>>(user, weightData);
            SetUserState<IEnumerable<ActivityDataPoint>>(user, activities);
            RaiseOnChangeEvent();
        }
    }
}
