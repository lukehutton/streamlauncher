using GalaSoft.MvvmLight.Command;
using StreamLauncher.Wpf.Infrastructure;

namespace StreamLauncher.Wpf.ViewModel
{
    public interface ILoginViewModel
    {
        AsyncRelayCommand<object> LoginCommand { get; }
        RelayCommand CancelCommand { get; }
        string ErrorMessage { get; set; }
        string UserName { get; set; }
        bool RememberMe { get; set; }
        bool? DialogResult { get; set; }
        string BusyText { get; set; }
        bool IsBusy { get; set; }
    }
}