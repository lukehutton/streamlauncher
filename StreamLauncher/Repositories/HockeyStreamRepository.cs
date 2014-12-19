using System.Collections.Generic;
using System.Threading.Tasks;
using StreamLauncher.Models;
using StreamLauncher.Providers;

namespace StreamLauncher.Repositories
{
    public class HockeyStreamRepository : IHockeyStreamRepository
    {
        private readonly IHockeyStreamsApiRequiringToken _hockeyStreamsApi;

        public HockeyStreamRepository(IHockeyStreamsApiRequiringToken hockeyStreamsApi)
        {
            _hockeyStreamsApi = hockeyStreamsApi;
        }

        public Task<IEnumerable<HockeyStream>> GetLiveStreams()
        {
            throw new System.NotImplementedException();
        }
    }
}