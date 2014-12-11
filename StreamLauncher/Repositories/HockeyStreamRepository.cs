using System.Collections.Generic;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public class HockeyStreamRepository : IHockeyStreamRepository
    {
        private readonly IEnumerable<HockeyStream> _inMemoryStreams;

        public HockeyStreamRepository()
        {
            _inMemoryStreams = new List<HockeyStream>
                {
                    new HockeyStream {Id = 1, EventType = EventType.Nhl, HomeTeam = "Vancouver Canucks", AwayTeam = "Boston Bruins"},
                    new HockeyStream {Id = 2, EventType = EventType.Nhl, HomeTeam = "Calgary Flames", AwayTeam = "Winnipeg Jets"},
                    new HockeyStream {Id = 3, EventType = EventType.Nhl, HomeTeam = "Chicago Blackhawks", AwayTeam = "New York Rangers"}
                };
        }

        public IEnumerable<HockeyStream> GetLiveStreams()
        {
            return _inMemoryStreams;
        }
    }
}