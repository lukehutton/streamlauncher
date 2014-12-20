using System.Collections.Generic;
using StreamLauncher.Models;

namespace StreamLauncher.Filters
{
    public abstract class HockeyStreamFilterSpecification
    {
        public IEnumerable<HockeyStream> Filter(IList<HockeyStream> hockeyStreams)
        {
            return ApplyFilter(hockeyStreams);
        }

        protected abstract IEnumerable<HockeyStream> ApplyFilter(IList<HockeyStream> hockeyStreams);
    }
}