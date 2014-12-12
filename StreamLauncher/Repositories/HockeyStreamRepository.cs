using System.Collections.Generic;
using System.Threading.Tasks;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public class HockeyStreamRepository : IHockeyStreamRepository
    {
        public Task<IEnumerable<HockeyStream>> GetLiveStreams()
        {
            throw new System.NotImplementedException();
        }
    }
}