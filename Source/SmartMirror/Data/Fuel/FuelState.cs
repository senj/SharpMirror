using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.Fuel
{
    public class FuelState
    {
        private readonly FuelService _fuelService;

        public FuelState(FuelService fuelService)
        {
            _fuelService = fuelService;
        }

        public event Action OnChange;

        public FuelResponse FuelResponse { get; private set; }

        public bool ShowDetails { get; private set; }

        public async Task<FuelResponse> GetFuelResponseAsync(int limit = 10, bool useCache = true)
        {
            FuelResponse = await _fuelService.GetFuelResponseAsync(limit, useCache);
            OnChange?.Invoke();

            return FuelResponse;
        }

        public void SetShowDetails(bool showDetails)
        {
            ShowDetails = showDetails;
            OnChange?.Invoke();
        }
    }
}
