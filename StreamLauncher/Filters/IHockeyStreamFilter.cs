using System.Collections.Generic;
using StreamLauncher.Models;

namespace StreamLauncher.Filters
{
    public interface IHockeyStreamFilter
    {
        IEnumerable<HockeyStream> By(IList<HockeyStream> hockeyStreams, HockeyStreamFilterSpecification filterSpecification);
    }
}