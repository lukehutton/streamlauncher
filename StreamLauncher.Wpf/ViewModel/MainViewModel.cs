using System;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Api;
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
        private readonly ITokenProvider _tokenProvider;

        private string _userName;
        private string _currentUser;
        private string _currentDate;

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand LogoutCommand { get; private set; }

        public RelayCommand<CancelEventArgs> Closing { get; private set; }

        public MainViewModel(IUserSettings userSettings, IAuthenticationService authenticationService, ITokenProvider tokenProvider)
        {
            _userSettings = userSettings;
            _authenticationService = authenticationService;
            _tokenProvider = tokenProvider;

            LoginCommand = new RelayCommand(HandleLoginCommand);
            LogoutCommand = new RelayCommand(HandleLogoutCommand);
            Closing = new RelayCommand<CancelEventArgs>(HandleClosingCommand);

            Messenger.Default.Register<LoginSuccessfulMessage>(this, LoginSuccessful);            
        }

        private void LoginSuccessful(LoginSuccessfulMessage loginSuccessful)
        {
            _tokenProvider.Token = loginSuccessful.AuthenticationResult.AuthenticatedUser.Token;
            _userName = loginSuccessful.AuthenticationResult.AuthenticatedUser.UserName;

            CurrentUser = string.Format("Hi {0}", _userName);
            CurrentDate = DateTime.Now.ToString("dddd, MMMM dd");

            Messenger.Default.Send(new AuthenticatedMessage
            {
                AuthenticationResult = loginSuccessful.AuthenticationResult
            });
        }        

        private void HandleClosingCommand(CancelEventArgs obj)
        {
            _userSettings.Save();
            Application.Current.Shutdown();
        }
        
        private void HandleLoginCommand()
        {
            OpenLoginDialog();
        }

        private static void OpenLoginDialog()
        {
            var loginViewModel = SimpleIoc.Default.GetInstance<LoginViewModel>(Guid.NewGuid().ToString());
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

        private void HandleLogoutCommand()
        {
            _tokenProvider.Token = string.Empty;

            _userSettings.UserName = string.Empty;
            _userSettings.EncryptedPassword = string.Empty;
            _userSettings.RememberMe = false;
            _userSettings.Save();            

            OpenLoginDialog();
        }

        public void AuthenticateUser()
        {
            if (IsInDesignModeStatic) return;

            if (!_userSettings.RememberMe)
            {
                HandleLoginCommand();
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
                HandleLoginCommand();
                return;
            }
            LoginSuccessful(new LoginSuccessfulMessage { AuthenticationResult = result });
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