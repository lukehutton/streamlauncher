using StreamLauncher.Util;

namespace StreamLauncher.Models
{
    public enum EventType
    {        
        AHL,
        NHL,
        OHL,
        WHL,
        WorldJuniors
    }

    public static class EventTypeParser
    {
        public static EventType Parse(string eventType)
        {
            if (eventType == "World Juniors") return EventType.WorldJuniors;
            return eventType.ParseEnum<EventType>();
        }
    }
}