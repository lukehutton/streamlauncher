using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StreamLauncher.Constants;
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

            var hockeyStreams = new List<HockeyStream>();
            foreach (var team in allTeams)
            {
                var feeds = schedule.FindAll(x => x.HomeTeam == team);
                var stream = MapHockeyStream(feeds);
                var timeWithoutTimeZone = stream.StartTime.Substring(0, stream.StartTime.LastIndexOf(' '));
                var startTime = DateTime.ParseExact(timeWithoutTimeZone, "h:mm tt", CultureInfo.InvariantCulture);
                stream.StartTimeSpan = startTime.TimeOfDay;
                hockeyStreams.Add(stream);
            }
            return hockeyStreams.OrderBy(x => x.StartTimeSpan);
        }

        private static string GetImagePathForTeam(string team)
        {
            return string.Format(@"../Images/Teams/{0}.png", team);
        }

        private static HockeyStream MapHockeyStream(List<LiveStreamDto> feeds)
        {
            var feed = feeds.First();
            var hockeyStream = new HockeyStream
            {
                Feeds = from f in feeds
                    select new Feed
                    {
                        Game = SetGame(f),
                        FeedType = SetFeedType(f),
                        StreamId = Convert.ToInt32(f.Id),
                        IsPlaying = f.IsPlaying == "1"
                    },
                HomeTeam = feed.HomeTeam.MaxStrLen(AppConstants.MaxTeamStringLength),
                AwayTeam = feed.AwayTeam.MaxStrLen(AppConstants.MaxTeamStringLength),
                StartTime = feed.StartTime,
                EventType = EventTypeParser.Parse(feed.Event),
                IsPlaying = feed.IsPlaying == "1",
                Score = feed.HomeScore + " - " + feed.AwayScore,
                HomeImagePath = GetImagePathForTeam(feed.HomeTeam),
                AwayImagePath = GetImagePathForTeam(feed.AwayTeam)
            };

            return hockeyStream;
        }

        private static string SetGame(LiveStreamDto f)
        {
            if (f.AwayTeam.IsNullOrEmpty())
            {
                return f.HomeTeam;
            }
            if (f.HomeTeam.IsNullOrEmpty())
            {
                return f.AwayTeam;
            }
            return "{0} at {1}".Fmt(f.AwayTeam, f.HomeTeam);
        }

        private static string SetFeedType(LiveStreamDto liveStreamDto)
        {
            if (liveStreamDto.FeedType.IsNullOrEmpty())
            {
                return "Play Feed";
            }

            switch (liveStreamDto.FeedType)
            {
                case "Home Feed":
                    return "{0} Feed".Fmt(liveStreamDto.HomeTeam);
                case "Away Feed":
                    return "{0} Feed".Fmt(liveStreamDto.AwayTeam);
                default:
                    return liveStreamDto.FeedType;
            }
        }
    }
}