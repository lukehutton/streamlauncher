﻿using System;
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
                var homeFeed = schedule.Find(x => x.HomeTeam == team && x.FeedType == "Home Feed");
                var awayFeed = schedule.Find(x => x.HomeTeam == team && x.FeedType == "Away Feed");
                // not home or away feeds i.e. NBC Feed or RDS Feed
                var noFeedTypes =
                    schedule.FindAll(
                        x =>
                            x.HomeTeam == team &&
                            (x.FeedType == null || (x.FeedType != "Home Feed" && x.FeedType != "Away Feed"))); 
                var stream = MapHockeyStream(homeFeed, awayFeed, noFeedTypes);
                if (stream != null)
                {
                    var timeWithoutTimeZone = stream.StartTime.Substring(0, stream.StartTime.LastIndexOf(' '));
                    var startTime = DateTime.ParseExact(timeWithoutTimeZone, "h:mm tt", CultureInfo.InvariantCulture);
                    stream.StartTimeSpan = startTime.TimeOfDay;
                    hockeyStreams.Add(stream);
                }
            }
            return hockeyStreams.OrderBy(x => x.StartTimeSpan);
        }

        private static string GetImagePathForTeam(string team)
        {
            return string.Format(@"../Images/Teams/{0}.png", team);
        }

        private static HockeyStream MapHockeyStream(LiveStreamDto homeFeed, LiveStreamDto awayFeed, List<LiveStreamDto> noFeedTypes)
        {            
            var hockeyStream = new HockeyStream();

            if (homeFeed != null)
            {
                hockeyStream.HomeStreamId = Convert.ToInt32(homeFeed.Id);
                hockeyStream.HomeTeam = homeFeed.HomeTeam.MaxStrLen(AppConstants.MaxTeamStringLength);
                hockeyStream.AwayTeam = homeFeed.AwayTeam.MaxStrLen(AppConstants.MaxTeamStringLength);
                hockeyStream.StartTime = homeFeed.StartTime;
                hockeyStream.EventType = EventTypeParser.Parse(homeFeed.Event);
                hockeyStream.IsPlaying = homeFeed.IsPlaying == "1";
                hockeyStream.Score = homeFeed.HomeScore + " - " + homeFeed.AwayScore;
                hockeyStream.HomeImagePath = GetImagePathForTeam(homeFeed.HomeTeam);
                hockeyStream.AwayImagePath = GetImagePathForTeam(homeFeed.AwayTeam);
                hockeyStream.PlayHomeFeedText = "Home Feed";
            }

            if (awayFeed != null)
            {
                hockeyStream.AwayStreamId = Convert.ToInt32(awayFeed.Id);
                hockeyStream.HomeTeam = awayFeed.HomeTeam.MaxStrLen(AppConstants.MaxTeamStringLength);
                hockeyStream.AwayTeam = awayFeed.AwayTeam.MaxStrLen(AppConstants.MaxTeamStringLength);
                hockeyStream.StartTime = awayFeed.StartTime;
                hockeyStream.EventType = EventTypeParser.Parse(awayFeed.Event);
                hockeyStream.IsPlaying = awayFeed.IsPlaying == "1";
                hockeyStream.Score = awayFeed.HomeScore + " - " + awayFeed.AwayScore;
                hockeyStream.HomeImagePath = GetImagePathForTeam(awayFeed.HomeTeam);
                hockeyStream.AwayImagePath = GetImagePathForTeam(awayFeed.AwayTeam);
                hockeyStream.PlayAwayFeedText = "Away Feed";
            }

            if (noFeedTypes.Count == 0)
            {
                return hockeyStream;
            }
                
            var noFeedType = noFeedTypes.First();
            switch (noFeedTypes.Count)
            {
                case 1:
                    if (hockeyStream.HomeStreamId == 0)
                    {
                        hockeyStream.HomeStreamId = Convert.ToInt32(noFeedType.Id);
                        hockeyStream.PlayHomeFeedText = SetPlayFeedText(noFeedType);
                    }                    
                    else if (hockeyStream.AwayStreamId == 0)
                    {
                        hockeyStream.AwayStreamId = Convert.ToInt32(noFeedType.Id);
                        hockeyStream.PlayAwayFeedText = SetPlayFeedText(noFeedType);
                    }
                    break;
                case 2:
                    var secondNoFeedType = noFeedTypes.ElementAt(1);
                    hockeyStream.HomeStreamId = Convert.ToInt32(noFeedType.Id);
                    hockeyStream.AwayStreamId = Convert.ToInt32(secondNoFeedType.Id);
                    hockeyStream.PlayHomeFeedText = SetPlayFeedText(noFeedType);
                    hockeyStream.PlayAwayFeedText = SetPlayFeedText(secondNoFeedType);
                    break;
            }

            hockeyStream.HomeTeam = noFeedType.HomeTeam.MaxStrLen(AppConstants.MaxTeamStringLength);
            hockeyStream.AwayTeam = noFeedType.AwayTeam.MaxStrLen(AppConstants.MaxTeamStringLength);
            hockeyStream.StartTime = noFeedType.StartTime;
            hockeyStream.EventType = EventTypeParser.Parse(noFeedType.Event);
            hockeyStream.IsPlaying = noFeedType.IsPlaying == "1";
            hockeyStream.Score = noFeedType.HomeScore + " - " + noFeedType.AwayScore;
            hockeyStream.HomeImagePath = GetImagePathForTeam(noFeedType.HomeTeam);
            hockeyStream.AwayImagePath = GetImagePathForTeam(noFeedType.AwayTeam);

            return hockeyStream;
        }

        private static string SetPlayFeedText(LiveStreamDto noFeedType)
        {
            return string.IsNullOrEmpty(noFeedType.FeedType) ? "Play Feed" : noFeedType.FeedType;
        }
    }
}