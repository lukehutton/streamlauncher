namespace StreamLauncher.Models
{
    public class HockeyStream
    {
        public int Id { get; set; }
        public EventType EventType { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
    }

    public enum EventType
    {
        Nhl
    }
}