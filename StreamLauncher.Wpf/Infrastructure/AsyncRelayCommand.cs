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
    
    public class AsyncRelayCommand<T> : RelayCommand<T>
    {
        private readonly Func<T, Task> _asyncExecute;
        private readonly Action<T> _execute;

        public AsyncRelayCommand(Func<T, Task> asyncExecute)
            : this(asyncExecute, T => asyncExecute(T))
        {
        }

        private AsyncRelayCommand(Func<T, Task> asyncExecute, Action<T> execute)
            : base(execute)
        {
            _asyncExecute = asyncExecute;
            _execute = execute;
        }

        public Task ExecuteAsync(T arg)
        {
            return _asyncExecute(arg);
        }

        public override void Execute(object parameter)
        {
            _asyncExecute((T)parameter);
        }
    }
}