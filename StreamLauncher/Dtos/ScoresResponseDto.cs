using System.Collections.Generic;

namespace StreamLauncher.Dtos
{
    public class ScoresResponseDto
    {
        public List<ScoreDto> Scores { get; set; }    
    }

    public class ScoreDto
    {
        public string Event { get; set; }
        public string HomeScore { get; set; }
        public string HomeTeam { get; set; }
        public string AwayScore { get; set; }
        public string AwayTeam { get; set; }
        public string Period { get; set; }
    }
}