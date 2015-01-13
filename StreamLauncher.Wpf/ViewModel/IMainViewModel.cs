using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.Messages;

namespace StreamLauncher.Wpf.ViewModel
{
    public interface IMainViewModel
    {
        RelayCommand LogoutCommand { get; }
        RelayCommand<CancelEventArgs> Closing { get; }
        string CurrentUser { get; set; }
        string CurrentDate { get; set; }
        string BusyText { get; set; }
        bool IsBusy { get; set; }
        void HandleAuthenticateMessage(AuthenticateMessage authenticateMessage);
        void HandleBusyStatusMessage(BusyStatusMessage busyStatusMessage);
        void HandleLoginSuccessfulMessage(LoginSuccessfulMessage loginSuccessful);
        void HandleClosingCommand(CancelEventArgs obj);
        void HandleLogoutCommand();
    }
}