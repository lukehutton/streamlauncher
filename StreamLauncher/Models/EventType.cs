using System;

namespace StreamLauncher.Models
{
    public enum EventType
    {        
        AHL,
        NHL,
        OHL,
        QMJHL,
        WHL,
        WorldJuniors,
        Uknown
    }

    public static class EventTypeParser
    {
        public static EventType Parse(string eventType)
        {
            if (eventType == "World Juniors") return EventType.WorldJuniors;
            EventType tryEventType;
            return Enum.TryParse(eventType, out tryEventType) ? tryEventType : EventType.Uknown;
        }
    }
}