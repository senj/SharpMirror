using System.Threading.Tasks;

namespace SmartMirror.Data.Soccer
{
    public class BundesligaState : Displayable
    {
        private readonly BundesligaService _bundesligaService;

        public BundesligaState(BundesligaService bundesligaService)
        {
            _bundesligaService = bundesligaService;
        }

        public BundesligaModel MatchResults { get; private set; }

        public async Task<BundesligaModel> GetCurrentPlayDayAsync()
        {
            MatchResults = await _bundesligaService.GetCurrentPlayDayAsync();
            RaiseOnChangeEvent();

            return MatchResults;
        }
    }
}
