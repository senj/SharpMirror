using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.GoogleFit
{
    public class GoogleFitState : Displayable
    {
        private readonly GoogleFitService _googleFitService;

        public GoogleFitState(GoogleFitService googleFitService)
        {
            _googleFitService = googleFitService;
        }

        public IEnumerable<WeightDataPoint> WeightData { get; private set; }
        public GoogleCodeResponse GoogleCodeResponse { get; private set; }
        public string Token { get; private set; }

        public async Task StartAuthorizationAsync()
        {
            GoogleCodeResponse =  await _googleFitService.StartAuthorizationAsync();
            RaiseOnChangeEvent();
        }

        public async Task AuthorizationPollingAsync(string user)
        {
            GoogleAuthResponse googleAuthResponse = await _googleFitService.AuthorizationPolling(user, GoogleCodeResponse);
            Token = googleAuthResponse?.access_token;
            RaiseOnChangeEvent();
        }
        
        public async Task<IEnumerable<WeightDataPoint>> GetWeight(int take)
        {
            WeightData = await _googleFitService.GetWeight(Token, take);
            RaiseOnChangeEvent();

            return WeightData;
        }
    }
}
