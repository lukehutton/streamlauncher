namespace StreamLauncher.Dtos
{
    public class GetLiveStreamsResponseDto
    {
        public string Id { get; set; }
        public string Event { get; set; }
        public string HomeTeam { get; set; }
        public string HomeScore { get; set; }
        public string AwayTeam { get; set; }
        public string AwayScore { get; set; }
        public string StartTime { get; set; }
        public string Period { get; set; }
        public bool IsHd { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsWmv { get; set; }
        public bool IsFlash { get; set; }
        public bool IsIStream { get; set; }
        public string FeedType { get; set; }
        public string SrcUrl { get; set; }
        public string HdUrl { get; set; }
        public string SdUrl { get; set; }
        public string TrueLiveSd { get; set; }
        public string TrueLiveHd { get; set; }
    }
}