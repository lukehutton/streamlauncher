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
                Id = index,
                EventType = EventType.Nhl,
                HomeTeam = "Home Team {0}".Fmt(index),
                AwayTeam = "Away Team {0} Canucks".Fmt(index)
            };
        }
    }
}