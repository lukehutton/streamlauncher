using System;
using System.Threading;
using System.Threading.Tasks;

namespace StreamLauncher.Util
{
    public class PeriodicTaskRunner : IPeriodicTaskRunner
    {
        public async Task Run(Action action, TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);
                action();
            }
        }

        public Task Run(Action action, TimeSpan period)
        {
            return Run(action, period, CancellationToken.None);
        }
    }
}