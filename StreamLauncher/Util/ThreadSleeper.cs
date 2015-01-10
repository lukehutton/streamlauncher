using System.Threading;

namespace StreamLauncher.Util
{
    public class ThreadSleeper : IThreadSleeper
    {
        public void SleepFor(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }
    }
}