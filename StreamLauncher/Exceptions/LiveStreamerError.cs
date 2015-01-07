using System;

namespace StreamLauncher.Exceptions
{
    public class LiveStreamerError : Exception
    {
        public LiveStreamerError(string message) : base(message)
        {
        }
    }
}