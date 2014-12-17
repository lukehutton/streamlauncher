using System.Collections.Generic;
using System.Linq;
using StreamLauncher.Models;

namespace StreamLauncher.Filters
{
    public class ActiveFilterSpecification : HockeyStreamFilterSpecification
    {
        private readonly bool _isPlaying;

        public ActiveFilterSpecification(bool isPlaying)
        {
            _isPlaying = isPlaying;
        }

        protected override IEnumerable<HockeyStream> ApplyFilter(IList<HockeyStream> hockeyStreams)
        {
            return hockeyStreams.Where(hockeyStream => _isPlaying == hockeyStream.IsPlaying);
        }
    }
}