using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace StreamLauncher.Wpf.Infrastructure
{
    public class AsyncRelayCommand : RelayCommand
    {
        private readonly Func<Task> _asyncExecute;
        private readonly Action _execute;

        public AsyncRelayCommand(Func<Task> asyncExecute)
            : this(asyncExecute, () => asyncExecute())
        {
        }

        private AsyncRelayCommand(Func<Task> asyncExecute, Action execute)
            : base(execute)
        {
            _asyncExecute = asyncExecute;
            _execute = execute;
        }

        public Task ExecuteAsync()
        {
            return _asyncExecute();
        }

        public override void Execute(object parameter)
        {
            _asyncExecute();
        }
    }
}