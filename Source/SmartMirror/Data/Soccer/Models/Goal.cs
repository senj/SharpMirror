namespace SmartMirror.Data.Soccer
{
    public class Goal
    {
        public int GoalID { get; set; }
        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }
        public int MatchMinute { get; set; }
        public int GoalGetterID { get; set; }
        public string GoalGetterName { get; set; }
        public bool IsPenalty { get; set; }
        public bool IsOwnGoal { get; set; }
        public bool IsOvertime { get; set; }
        public object Comment { get; set; }
    }
}