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
        private const int MaxTeamStringLength = 17;

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
                    HomeTeam = noFeedType.HomeTeam.MaxStrLen(MaxTeamStringLength),
                    AwayTeam = noFeedType.AwayTeam.MaxStrLen(MaxTeamStringLength),
                    StartTime = noFeedType.StartTime,
                    EventType = EventTypeParser.Parse(noFeedType.Event),
                    IsPlaying = noFeedType.IsPlaying == "1",
                    Score = noFeedType.HomeScore + " - " + noFeedType.AwayScore,
                    HomeImagePath = GetImagePathForTeam(noFeedType.HomeTeam),
                    AwayImagePath = GetImagePathForTeam(noFeedType.AwayTeam),
                    PlayHomeFeedImagePath = GetImagePathForPlayFeed(FeedType.None)
                };     
            }

            var hockeyStream = new HockeyStream();

            if (homeFeed != null)
            {
                hockeyStream.HomeStreamId = Convert.ToInt32(homeFeed.Id);
                hockeyStream.HomeTeam = homeFeed.HomeTeam.MaxStrLen(MaxTeamStringLength);
                hockeyStream.AwayTeam = homeFeed.AwayTeam.MaxStrLen(MaxTeamStringLength);
                hockeyStream.StartTime = homeFeed.StartTime;
                hockeyStream.EventType = EventTypeParser.Parse(homeFeed.Event);
                hockeyStream.IsPlaying = homeFeed.IsPlaying == "1";
                hockeyStream.Score = homeFeed.HomeScore + " - " + homeFeed.AwayScore;
                hockeyStream.HomeImagePath = GetImagePathForTeam(homeFeed.HomeTeam);
                hockeyStream.AwayImagePath = GetImagePathForTeam(homeFeed.AwayTeam);
                hockeyStream.PlayHomeFeedImagePath = GetImagePathForPlayFeed(FeedType.Home);
            }

            if (awayFeed != null)
            {
                hockeyStream.AwayStreamId = Convert.ToInt32(awayFeed.Id);
                hockeyStream.HomeTeam = awayFeed.HomeTeam.MaxStrLen(MaxTeamStringLength);
                hockeyStream.AwayTeam = awayFeed.AwayTeam.MaxStrLen(MaxTeamStringLength);
                hockeyStream.StartTime = awayFeed.StartTime;
                hockeyStream.EventType = EventTypeParser.Parse(awayFeed.Event);
                hockeyStream.IsPlaying = awayFeed.IsPlaying == "1";
                hockeyStream.Score = awayFeed.HomeScore + " - " + awayFeed.AwayScore;
                hockeyStream.HomeImagePath = GetImagePathForTeam(awayFeed.HomeTeam);
                hockeyStream.AwayImagePath = GetImagePathForTeam(awayFeed.AwayTeam);
                hockeyStream.PlayAwayFeedImagePath = GetImagePathForPlayFeed(FeedType.Away);
            }            

            return hockeyStream.HomeStreamId == 0 && hockeyStream.AwayStreamId == 0 ? null : hockeyStream;
        }

        private static string GetImagePathForPlayFeed(FeedType feedType)
        {
            return string.Format(@"../Images/Buttons/{0}.png", feedType);
        }
    }
}