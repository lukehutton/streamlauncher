namespace StreamLauncher.Models
{
    public class HockeyStream
    {
        public int HomeStreamId { get; set; }
        public int AwayStreamId { get; set; }
        public EventType EventType { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Score { get; set; }
        public string StartTime { get; set; }
        public string Period { get; set; }
        public bool IsPlaying { get; set; }
    }

    public enum EventType
    {
        Nhl
    }
}