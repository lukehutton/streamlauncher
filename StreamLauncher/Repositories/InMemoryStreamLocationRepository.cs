using System.Collections.Generic;
using StreamLauncher.Models;
using StreamLauncher.Util;

namespace StreamLauncher.Repositories
{
    public class InMemoryStreamLocationRepository : IStreamLocationRepository
    {
        public IEnumerable<StreamLocation> GetLocations()
        {
            var result = new List<StreamLocation>();
            for (var index = 0; index < 5; index++)
            {
                result.Add(GetStreamLocation(index));
            }
            return result;
        }

        private StreamLocation GetStreamLocation(int index)
        {
            return new StreamLocation
            {
                Location = "Location {0}".Fmt(index)
            };
        }
    }
}