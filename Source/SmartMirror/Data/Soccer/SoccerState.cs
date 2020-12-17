using System.Threading.Tasks;

namespace SmartMirror.Data.Soccer
{
    public class SoccerState : Displayable
    {
        private readonly SoccerService _soccerService;

        public SoccerState(SoccerService soccerService)
        {
            _soccerService = soccerService;
        }

        public BundesligaModel MatchResults { get; private set; }

        public async Task<BundesligaModel> GetCurrentPlayDayAsync()
        {
            MatchResults = await _soccerService.GetCurrentPlayDayAsync();
            RaiseOnChangeEvent();

            return MatchResults;
        }
    }
}
