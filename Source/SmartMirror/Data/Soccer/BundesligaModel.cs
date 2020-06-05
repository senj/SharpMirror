using System;
using System.Collections.Generic;

namespace SmartMirror.Data.Soccer
{
    public class BundesligaModel : List<Match>
    {
        public Match[] Matches { get; set; }
    }

    public class Match
    {
        public int MatchID { get; set; }
        public DateTime MatchDateTime { get; set; }
        public string TimeZoneID { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public DateTime MatchDateTimeUTC { get; set; }
        public Group Group { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool MatchIsFinished { get; set; }
        public Matchresult[] MatchResults { get; set; }
        public Goal[] Goals { get; set; }
        public object Location { get; set; }
        public object NumberOfViewers { get; set; }
    }
}