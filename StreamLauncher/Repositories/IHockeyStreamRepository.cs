using System.Collections.Generic;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public interface IHockeyStreamRepository
    {
        IEnumerable<HockeyStream> GetLiveStreams();
    }
}