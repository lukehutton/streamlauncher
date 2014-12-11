using System.Collections.Generic;
using System.Linq;
using StreamLauncher.Models;

namespace StreamLauncher.Filters
{
    public class EventTypeFilterSpecification : HockeyStreamFilterSpecification
    {
        private readonly EventType _eventType;

        public EventTypeFilterSpecification(EventType eventType)
        {
            _eventType = eventType;
        }

        protected override IEnumerable<HockeyStream> ApplyFilter(IList<HockeyStream> hockeyStreams)
        {
            return hockeyStreams.Where(hockeyStream => hockeyStream.EventType == _eventType);
        }
    }
}