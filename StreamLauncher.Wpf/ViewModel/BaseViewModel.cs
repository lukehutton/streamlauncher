using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace StreamLauncher.Wpf.ViewModel
{
    public class BaseViewModel : ViewModelBase
    {
        private string _busyText;
        private bool _isBusy;

        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        public Thread ActiveThread { get; set; }

        public void ExecuteActionInBackground(Action workerMethod)
        {
            IsBusy = true;

            var worker = new BackgroundWorker();
            worker.DoWork += delegate { workerMethod(); };
            worker.RunWorkerCompleted += delegate { IsBusy = false; };
            worker.RunWorkerAsync();
        }

        public async void ExecuteAsyncActionInBackground(Action action)
        {
            IsBusy = true;
            var task = Task.Factory.StartNew(action);
            await task.ContinueWith(_ => { IsBusy = false; });
        }
    }
}