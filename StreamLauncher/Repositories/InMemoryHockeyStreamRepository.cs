using System.Collections.Generic;
using System.Threading.Tasks;
using StreamLauncher.Models;
using StreamLauncher.Util;

namespace StreamLauncher.Repositories
{
    public class InMemoryHockeyStreamRepository : IHockeyStreamRepository
    {        
        public Task<IEnumerable<HockeyStream>> GetLiveStreams()
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
                HomeStreamId = index,
                AwayStreamId = index,
                EventType = EventType.NHL,
                HomeImagePath = @"../Images/Teams/Vancouver.png",                
                HomeTeam = "Home Team {0}".Fmt(index),
                AwayTeam = "Away Team {0}".Fmt(index),
                AwayImagePath = @"../Images/Teams/Toronto.png",
                Score = "1 - 0",
                StartTime = "7:30 PM PST",
                Period = "1",                
                IsPlaying = true                
            };
        }
    }
}