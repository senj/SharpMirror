using System.Threading.Tasks;

namespace SmartMirror.Data.Soccer
{
    public class SoccerState : StateBase
    {
        private readonly SoccerService _soccerService;

        public SoccerState(SoccerService soccerService) : base("Soccer", typeof(Shared.Bundesliga))
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
