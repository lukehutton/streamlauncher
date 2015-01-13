using GalaSoft.MvvmLight.Command;

namespace StreamLauncher.Wpf.ViewModel
{
    public interface ILoginViewModel
    {
        RelayCommand<object> LoginCommand { get; }
        RelayCommand CancelCommand { get; }
        string ErrorMessage { get; set; }
        string UserName { get; set; }
        bool RememberMe { get; set; }
        bool? DialogResult { get; set; }
        string BusyText { get; set; }
        bool IsBusy { get; set; }
        void HandleCancelCommand();
        void HandleLoginCommand(object parameter);
    }
}