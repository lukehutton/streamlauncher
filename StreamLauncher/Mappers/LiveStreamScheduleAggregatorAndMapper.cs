using System;
using System.Collections.Generic;
using System.Linq;
using StreamLauncher.Dtos;
using StreamLauncher.Models;

namespace StreamLauncher.Mappers
{
    public class LiveStreamScheduleAggregatorAndMapper : ILiveStreamScheduleAggregatorAndMapper
    {
        public IEnumerable<HockeyStream> AggregateAndMap(GetLiveStreamsResponseDto getLiveStreamsResponseDto)
        {
            var schedule = getLiveStreamsResponseDto.Schedule;
            
            var allTeams = schedule.Select(x => x.HomeTeam).Distinct();

            var hockeyStreams = new List<HockeyStream>();
            foreach (var team in allTeams)
            {
                var homeFeed = schedule.Find(x => x.HomeTeam == team && x.FeedType == "Home Feed");
                var awayFeed = schedule.Find(x => x.HomeTeam == team && x.FeedType == "Away Feed");
                var noFeedType = schedule.Find(x => x.HomeTeam == team && x.FeedType == null);
                var hockeyStream = MapHockeyStream(homeFeed, awayFeed, noFeedType);
                if (hockeyStream != null)
                {
                    hockeyStreams.Add(hockeyStream);
                }
            }
            return hockeyStreams.OrderBy(x => x.StartTime);
        }

        private static string GetImagePathForTeam(string team)
        {
            return string.Format(@"../Images/Teams/{0}.png", team);
        }

        private static HockeyStream MapHockeyStream(LiveStreamDto homeFeed, LiveStreamDto awayFeed, LiveStreamDto noFeedType)
        {            
            if (noFeedType != null)
            {
                return new HockeyStream
                {
                    NoFeedTypeStreamId = Convert.ToInt32(noFeedType.Id),
                    HomeTeam = noFeedType.HomeTeam,
                    AwayTeam = noFeedType.AwayTeam,
                    StartTime = noFeedType.StartTime,
                    EventType = EventTypeParser.Parse(noFeedType.Event),
                    IsPlaying = noFeedType.IsPlaying == "1",
                    Score = noFeedType.HomeScore + " - " + noFeedType.AwayScore,
                    HomeImagePath = GetImagePathForTeam(noFeedType.HomeTeam),
                    AwayImagePath = GetImagePathForTeam(noFeedType.AwayTeam)
                };     
            }

            var hockeyStream = new HockeyStream();

            if (homeFeed != null)
            {
                hockeyStream.HomeStreamId = Convert.ToInt32(homeFeed.Id);
                hockeyStream.HomeTeam = homeFeed.HomeTeam;
                hockeyStream.AwayTeam = homeFeed.AwayTeam;
                hockeyStream.StartTime = homeFeed.StartTime;
                hockeyStream.EventType = EventTypeParser.Parse(homeFeed.Event);
                hockeyStream.IsPlaying = homeFeed.IsPlaying == "1";
                hockeyStream.Score = homeFeed.HomeScore + " - " + homeFeed.AwayScore;
                hockeyStream.HomeImagePath = GetImagePathForTeam(homeFeed.HomeTeam);
                hockeyStream.AwayImagePath = GetImagePathForTeam(homeFeed.AwayTeam);
            }

            if (awayFeed != null)
            {
                hockeyStream.AwayStreamId = Convert.ToInt32(awayFeed.Id);
                hockeyStream.HomeTeam = awayFeed.HomeTeam;
                hockeyStream.AwayTeam = awayFeed.AwayTeam;
                hockeyStream.StartTime = awayFeed.StartTime;
                hockeyStream.EventType = EventTypeParser.Parse(awayFeed.Event);
                hockeyStream.IsPlaying = awayFeed.IsPlaying == "1";
                hockeyStream.Score = awayFeed.HomeScore + " - " + awayFeed.AwayScore;
                hockeyStream.HomeImagePath = GetImagePathForTeam(awayFeed.HomeTeam);
                hockeyStream.AwayImagePath = GetImagePathForTeam(awayFeed.AwayTeam);
            }

            return hockeyStream.HomeStreamId == 0 && hockeyStream.AwayStreamId == 0 ? null : hockeyStream;
        }
    }
}