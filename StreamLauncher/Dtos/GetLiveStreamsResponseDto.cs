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
        //todo add remaining fields
//feedType:<Home Feed, Away Feed, null>,
//srcUrl:<source_url or null (only shows when streams are started)>,
//hdUrl:<source_url or null (only shows when streams are started)>,
//sdUrl:<source_url or null (only shows when streams are started)>,
//TrueLiveSD:<source_url or null (only shows when streams are started)>,
//TrueLiveHD:<source_url or null (only shows when streams are started)> 
    }
}