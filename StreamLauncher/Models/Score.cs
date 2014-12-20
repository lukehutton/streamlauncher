namespace StreamLauncher.Models
{
    public class Score
    {
        public EventType EventType { get; set; }
        public string HomeTeam { get; set; }
        public int HomeScore { get; set; }
        public string AwayTeam { get; set; }
        public int AwayScore { get; set; }
        public string PeriodAndTimeLeft { get; set; }        
    }
}