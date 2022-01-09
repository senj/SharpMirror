using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.Fitbit
{
    public class FitbitState : StateBase
    {
        private readonly FitbitService _fitbitService;

        public FitbitState(FitbitService fitbitService) : base("Fitbit", typeof(Shared.Fitbit))
        {
            _fitbitService = fitbitService;
        }
        
        public FitbitUserResponse FitbitUserResponse { get; private set; }
        public FitbitSleepResponse FitbitSleepResponse { get; private set; }
        public FitbitDeviceResponse FitbitDeviceResponse { get; private set; }
        public Dictionary<string, ActiveMinutes> ActiveMinutes { get; private set; }

        public async Task PopulateDataAsync(string token, DateTime sleepDate)
        {
            if (string.IsNullOrEmpty(token)) return;

            FitbitUserResponse = await _fitbitService.GetUserInfoAsync(token);
            FitbitSleepResponse = await _fitbitService.GetSleepOfAsync(token, sleepDate);
            FitbitDeviceResponse = await _fitbitService.GetDeviceAsync(token);

            RaiseOnChangeEvent();
        }

        public async Task PopulateActiveMinutesAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return;

            ActiveMinutes = new Dictionary<string, ActiveMinutes>();
            AddToDict("high", await _fitbitService.GetActiveMinutes("minutesVeryActive", token));
            AddToDict("mid", await _fitbitService.GetActiveMinutes("minutesFairlyActive", token));
            AddToDict("low", await _fitbitService.GetActiveMinutes("minutesLightlyActive", token));
            AddToDict("verylow", await _fitbitService.GetActiveMinutes("minutesSedentary", token));

            RaiseOnChangeEvent();
        }

        private void AddToDict(string key, ActiveMinutes activeMinutes)
        {
            if (ActiveMinutes.ContainsKey(key))
            {
                ActiveMinutes[key] = activeMinutes;
            }
            else
            {
                ActiveMinutes.Add(key, activeMinutes);
            }
        }
    }
}
