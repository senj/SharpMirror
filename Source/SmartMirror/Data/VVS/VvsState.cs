using System.Threading.Tasks;

namespace SmartMirror.Data.VVS
{
    public class VvsState : StateBase
    {
        private readonly VvsService _vvsService;

        public VvsState(VvsService vvsService) : base("Vvs", typeof(Shared.Vvs))
        {
            _vvsService = vvsService;
        }

        public VvsResponse VvsResponse { get; private set; }
        public int[] Filter { get; set; }
        public int Limit { get; set; }

        public async Task<VvsResponse> GetVvsResponseAsync(int limit, int[] filter)
        {
            Limit = limit;
            Filter = filter;

            VvsResponse = await _vvsService.GetVvsResponseAsync(limit, filter);
            RaiseOnChangeEvent();

            return VvsResponse;
        }

        public void SetFilter(int[] filter)
        {
            Filter = filter;
        }

        public void SetLimit(int limit)
        {
            Limit = limit;
        }
    }
}
