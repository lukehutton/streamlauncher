using System;

namespace StreamLauncher.Exceptions
{
    public class LiveStreamerExecutableNotFound : Exception
    {
        public LiveStreamerExecutableNotFound(string message) : base(message)
        {
        }
    }
}