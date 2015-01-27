using System;

namespace StreamLauncher.Exceptions
{
    public class MediaPlayerNotFound : Exception
    {
        public MediaPlayerNotFound(string message) : base(message)
        {
        }
    }
}