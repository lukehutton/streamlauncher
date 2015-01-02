using System;

namespace StreamLauncher.Exceptions
{
    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException(string message) : base(message) { }
    }
}