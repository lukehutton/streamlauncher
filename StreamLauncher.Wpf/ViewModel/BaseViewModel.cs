using System;
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

        public async void ExecuteInBackground(Action action, Action completedAction)
        {            
            var task = Task.Factory.StartNew(action);
            await task.ContinueWith(_ =>
            {
                completedAction();
            }, TaskScheduler.FromCurrentSynchronizationContext());        
        }
    }
}