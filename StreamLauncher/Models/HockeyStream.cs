using System;

namespace StreamLauncher.Models
{
    public class HockeyStream
    {        
        public int HomeStreamId { get; set; }       
        public int AwayStreamId { get; set; }
        public EventType EventType { get; set; }
        public string HomeImagePath { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string AwayImagePath { get; set; }
        public string Score { get; set; }
        public string StartTime { get; set; }
        public string PeriodAndTimeLeft { get; set; }
        public bool IsPlaying { get; set; }
        public string PlayHomeFeedText { get; set; }
        public string PlayAwayFeedText { get; set; }
        public TimeSpan StartTimeSpan { get; set; }
        public bool IsFavorite { get; set; }
    }
}