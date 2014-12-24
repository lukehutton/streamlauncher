using System;
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
        private string _userName;

        private string _currentUser;
        private string _currentDate;

        public RelayCommand OpenLoginDialogCommand { get; private set; }
        public RelayCommand<CancelEventArgs> Closing { get; private set; }
        
        public MainViewModel(IUserSettings userSettings, IAuthenticationService authenticationService)
        {
            _userSettings = userSettings;
            _authenticationService = authenticationService;

            OpenLoginDialogCommand = new RelayCommand(OpenLoginWindow);
            Closing = new RelayCommand<CancelEventArgs>(CloseApp);

            Messenger.Default.Register<LoginSuccessfulMessage>(this, LoginSuccessful);            
        }

        private void LoginSuccessful(LoginSuccessfulMessage loginSuccessful)
        {
            _userName = loginSuccessful.AuthenticationResult.AuthenticatedUser.UserName;
            Messenger.Default.Send(new AuthenticatedMessage
            {
                AuthenticationResult = loginSuccessful.AuthenticationResult
            });
            SetCurrentUser();
            SetCurrentDate();
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

        public void AuthenticateUser()
        {
            if (IsInDesignModeStatic) return;

            if (!_userSettings.RememberMe)
            {
                OpenLoginWindow();
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
                Messenger.Default.Send(new AutoLoginFailedMessage
                {
                    UserName = _userSettings.UserName,                    
                    ErrorMessage = result.ErrorMessage                    
                });
                OpenLoginWindow();
                return;
            }
            LoginSuccessful(new LoginSuccessfulMessage { AuthenticationResult = result });
        }

        private void SetCurrentUser()
        {
            CurrentUser = string.Format("Hi {0}", _userName);
        }

        private void SetCurrentDate()
        {
            CurrentDate = DateTime.Now.ToString("dddd, MMMM dd");
        }

        public string CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                RaisePropertyChanged();
            }
        }
    }
}