using System;
using System.Threading;
using System.Threading.Tasks;

namespace StreamLauncher.Util
{
    public interface IPeriodicTaskRunner
    {
        Task Run(Action action, TimeSpan period, CancellationToken cancellationToken);
        Task Run(Action action, TimeSpan period);
    }
}