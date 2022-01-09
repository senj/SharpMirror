using System.Threading.Tasks;

namespace SmartMirror.Data.Fuel
{
    public class FuelState : StateBase
    {
        private readonly FuelService _fuelService;

        public FuelState(FuelService fuelService) : base("Fuel", typeof(Shared.Fuel))
        {
            _fuelService = fuelService;
        }

        public FuelResponse FuelResponse { get; private set; }

        public async Task<FuelResponse> GetFuelResponseAsync(int limit = 5, bool useCache = true)
        {
            FuelResponse = await _fuelService.GetFuelResponseAsync(limit, useCache);
            RaiseOnChangeEvent();

            return FuelResponse;
        }
    }
}
