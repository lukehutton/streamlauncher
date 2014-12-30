using System.Collections.Generic;
using System.Linq;
using StreamLauncher.Models;

namespace StreamLauncher.Filters
{
    public class ActiveFilterSpecification : HockeyStreamFilterSpecification
    {
        private readonly string _activeState;

        public ActiveFilterSpecification(string activeState)
        {
            _activeState = activeState;
        }

        protected override IEnumerable<HockeyStream> ApplyFilter(IList<HockeyStream> hockeyStreams)
        {
            switch (_activeState)
            {
                case "In Progress" : return hockeyStreams.Where(hockeyStream => hockeyStream.IsPlaying);
                case "Coming Soon": return hockeyStreams.Where(hockeyStream => hockeyStream.PeriodAndTimeLeft == "-");
                case "Completed": return hockeyStreams.Where(hockeyStream => hockeyStream.PeriodAndTimeLeft == "Final");
            }
            return hockeyStreams;            
        }
    }
}