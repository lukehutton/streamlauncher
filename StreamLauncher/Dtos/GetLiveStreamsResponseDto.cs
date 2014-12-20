﻿using System.Collections.Generic;

namespace StreamLauncher.Dtos
{
    public class GetLiveStreamsResponseDto
    {
        public List<LiveStreamDto> Schedule { get; set; }
    }

    public class LiveStreamDto
    {
        public string Id { get; set; }
        public string Event { get; set; }
        public string HomeTeam { get; set; }
        public string HomeScore { get; set; }
        public string AwayTeam { get; set; }
        public string AwayScore { get; set; }
        public string StartTime { get; set; }
        public string Period { get; set; }
        public string IsHd { get; set; }
        public string IsPlaying { get; set; }
        public string IsWmv { get; set; }
        public string IsFlash { get; set; }
        public string IsIStream { get; set; }
        public string FeedType { get; set; }
        public string SrcUrl { get; set; }
        public string HdUrl { get; set; }
        public string SdUrl { get; set; }
        public string TrueLiveSd { get; set; }
        public string TrueLiveHd { get; set; }
    }
}