using System;
using System.Collections.Generic;
using System.Linq;
using StreamLauncher.Dtos;
using StreamLauncher.Models;
using StreamLauncher.Util;

namespace StreamLauncher.Mappers
{
    public class LiveStreamScheduleAggregatorAndMapper : ILiveStreamScheduleAggregatorAndMapper
    {
        public IEnumerable<HockeyStream> AggregateAndMap(GetLiveStreamsResponseDto getLiveStreamsResponseDto)
        {
            var schedule = getLiveStreamsResponseDto.Schedule;
            
            var allTeams = schedule.Select(x => x.HomeTeam).Distinct();

            var hockeyStreams = from team
                in allTeams
                let homeFeed = schedule.Find(x => x.HomeTeam == team && x.FeedType == "Home Feed")
                let awayFeed = schedule.Find(x => x.HomeTeam == team && x.FeedType == "Away Feed")
                select new HockeyStream
                {
                    HomeTeam = homeFeed.HomeTeam,
                    HomeStreamId = Convert.ToInt32(homeFeed.Id),
                    AwayTeam = awayFeed.AwayTeam,
                    AwayStreamId = Convert.ToInt32(awayFeed.Id),
                    StartTime = homeFeed.StartTime,
                    EventType = homeFeed.Event.ParseEnum<EventType>(),
                    IsPlaying = homeFeed.IsPlaying == "1",
                    Score = homeFeed.HomeScore + " - " + homeFeed.AwayScore,
                    HomeImagePath = GetImagePathForTeam(homeFeed.HomeTeam),
                    AwayImagePath = GetImagePathForTeam(homeFeed.AwayTeam)
                };

            return hockeyStreams.OrderBy(x => x.StartTime);
        }

        private static string GetImagePathForTeam(string team)
        {
            return string.Format(@"../Images/Teams/{0}.png", team);
        }
    }
}