using System.Collections.Generic;
using StreamLauncher.Models;

namespace StreamLauncher.Filters
{
    public class HockeyStreamFilter : IHockeyStreamFilter
    {
        public IEnumerable<HockeyStream> By(IList<HockeyStream> hockeyStreams, HockeyStreamFilterSpecification filterSpecification)
        {
            return filterSpecification.Filter(hockeyStreams);
        }
    }
}