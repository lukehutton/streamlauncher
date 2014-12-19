namespace StreamLauncher.Dtos
{
    public class GetLiveDto
    {
        public string Id { get; set; }
        public string Event { get; set; }
        public string HomeTeam { get; set; }
        public string HomeScore { get; set; }

        //todo add remaining fields
//awayTeam:<away team name>,
//awayScore:<away team score>,
//startTime:<event/game start time>,
//period:<period of game>,
//isHd:<0 = no, 1 = yes>,
//isPlaying:<0 = not playing, 1 = currently started>,
//isWMV:<0 = no, 1 = yes>,
//isFlash:<0 = no, 1 = yes>,
//isiStream:<0 = no, 1 = yes>,
//feedType:<Home Feed, Away Feed, null>,
//srcUrl:<source_url or null (only shows when streams are started)>,
//hdUrl:<source_url or null (only shows when streams are started)>,
//sdUrl:<source_url or null (only shows when streams are started)>,
//TrueLiveSD:<source_url or null (only shows when streams are started)>,
//TrueLiveHD:<source_url or null (only shows when streams are started)> 
    }
}