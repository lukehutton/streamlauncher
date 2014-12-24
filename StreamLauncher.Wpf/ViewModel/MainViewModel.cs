using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Authentication;
using StreamLauncher.Repositories;
using StreamLauncher.Security;
using StreamLauncher.Wpf.Messages;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Wpf.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IUserSettings _userSettings;
        private readonly IAuthenticationService _authenticationService;

        public RelayCommand OpenLoginDialogCommand { get; private set; }
        public RelayCommand<CancelEventArgs> Closing { get; private set; }

        public MainViewModel(IUserSettings userSettings, IAuthenticationService authenticationService)
        {
            _userSettings = userSettings;
            _authenticationService = authenticationService;

            OpenLoginDialogCommand = new RelayCommand(OpenLoginWindow);
            Closing = new RelayCommand<CancelEventArgs>(CloseApp);

            AuthenticateUser();
        }

        private void CloseApp(CancelEventArgs obj)
        {
            _userSettings.Save();
            Application.Current.Shutdown();
        }
        
        private void OpenLoginWindow()
        {
            var loginViewModel = SimpleIoc.Default.GetInstance<LoginViewModel>();
            var loginWindow = new LoginWindow
            {
                DataContext = loginViewModel
            };
            var authenticated = loginWindow.ShowDialog() ?? false;
            if (!authenticated)
            {                
                Application.Current.Shutdown();
            }
        }

        private void AuthenticateUser()
        {
            if (IsInDesignModeStatic) return;

            if (!_userSettings.RememberMe)
            {
                OpenLoginDialogCommand.Execute(null);
                return;
            }
            string password;
            using (var secureString = _userSettings.EncryptedPassword.DecryptString())
            {
                password = secureString.ToInsecureString();
            }
            var result = _authenticationService.Authenticate(_userSettings.UserName, password);
            if (!result.IsAuthenticated)
            {
                Messenger.Default.Send(new LoginMessage
                {
                    UserName = _userSettings.UserName,
                    Password = password,
                    ErrorMessage = result.ErrorMessage,
                    RememberMe = true
                });
                OpenLoginDialogCommand.Execute(null);
            }
            Messenger.Default.Send(new AuthenticatedMessage
            {
                AuthenticationResult = result
            });
        }
    }
}