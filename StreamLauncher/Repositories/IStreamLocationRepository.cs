using System.Collections.Generic;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public interface IStreamLocationRepository
    {
        IEnumerable<StreamLocation> GetLocations();
    }
}