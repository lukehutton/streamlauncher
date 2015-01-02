using System;

namespace StreamLauncher.Exceptions
{
    public class HockeyStreamsApiBadRequest : Exception
    {
        public HockeyStreamsApiBadRequest(string message) : base(message) {}
    }
}