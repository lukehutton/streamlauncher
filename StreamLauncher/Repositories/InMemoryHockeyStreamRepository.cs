using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamLauncher.Models;
using StreamLauncher.Util;

namespace StreamLauncher.Repositories
{
    public class InMemoryHockeyStreamRepository : IHockeyStreamRepository
    {        
        public Task<IEnumerable<HockeyStream>> GetLiveStreams(DateTime date)
        {
            var result = new List<HockeyStream>();
            for (var index = 0; index < 15; index++)
            {
                result.Add(GetHockeyStream(index));
            }
            return Task.FromResult<IEnumerable<HockeyStream>>(result);
        }

        private HockeyStream GetHockeyStream(int index)
        {
            return new HockeyStream
            {
                NoFeedTypeStreamId = 0,
                HomeStreamId = index,
                AwayStreamId = index,
                EventType = EventType.NHL,
                HomeImagePath = @"../Images/Teams/Vancouver Canucks.png",                
                HomeTeam = "Home Team {0}".Fmt(index),
                AwayTeam = "Away Team {0}".Fmt(index),
                AwayImagePath = @"../Images/Teams/Toronto Maple Leafs.png",
                Score = "1 - 0",
                StartTime = "7:30 PM PST",
                PeriodAndTimeLeft = "13:20 1st",                
                IsPlaying = true                
            };
        }
    }
}