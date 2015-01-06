using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Api;
using StreamLauncher.Authentication;
using StreamLauncher.MediaPlayers;
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
        private readonly ILiveStreamer _liveStreamer;

        private string _userName;
        private string _currentUser;
        private string _currentDate;

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand LogoutCommand { get; private set; }

        public RelayCommand<CancelEventArgs> Closing { get; private set; }

        public MainViewModel(IUserSettings userSettings,
            IAuthenticationService authenticationService,
            ITokenProvider tokenProvider,
            ILiveStreamer liveStreamer)
        {
            _userSettings = userSettings;
            _authenticationService = authenticationService;
            _tokenProvider = tokenProvider;
            _liveStreamer = liveStreamer;

            LoginCommand = new RelayCommand(HandleLoginCommand);
            LogoutCommand = new RelayCommand(HandleLogoutCommand);
            Closing = new RelayCommand<CancelEventArgs>(HandleClosingCommand);

            Messenger.Default.Register<LoginSuccessfulMessage>(this, HandleLoginSuccessfulMessage);            
        }

        private void HandleLoginSuccessfulMessage(LoginSuccessfulMessage loginSuccessful)
        {
            _tokenProvider.Token = loginSuccessful.AuthenticationResult.AuthenticatedUser.Token;
            _userName = loginSuccessful.AuthenticationResult.AuthenticatedUser.UserName;

            BootstrapApp();

            Messenger.Default.Send(new AuthenticatedMessage
            {
                AuthenticationResult = loginSuccessful.AuthenticationResult
            });
        }

        private void BootstrapApp()
        {
            CurrentUser = string.Format("Hi {0}", _userName);
            CurrentDate = DateTime.Now.ToString("dddd, MMMM dd");

            if (_userSettings.IsFirstRun)
            {
                _userSettings.LiveStreamerPath = Environment.Is64BitOperatingSystem
                    ? LiveStreamer.Default64BitLocation
                    : LiveStreamer.Default32BitLocation;
                _userSettings.Save();                

                _userSettings.MediaPlayerPath = Environment.Is64BitOperatingSystem
                    ? Vlc.Default64BitLocation
                    : Vlc.Default32BitLocation;

                _userSettings.MediaPlayerArguments = Vlc.DefaultArgs;

                _userSettings.IsFirstRun = false;
                _userSettings.Save();
                _liveStreamer.SaveConfig();
                
                //todo show setting dialog via message
            }
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
            Messenger.Default.Send(new NotificationMessage(this, "HideMainWindow"));

            _tokenProvider.Token = string.Empty;

            _userSettings.UserName = string.Empty;
            _userSettings.EncryptedPassword = string.Empty;
            _userSettings.RememberMe = false;
            _userSettings.Save();            

            OpenLoginDialog();

            Messenger.Default.Send(new NotificationMessage(this, "ShowMainWindow"));
        }

        public void AuthenticateUser()
        {
            if (IsInDesignModeStatic) return;

#if DEBUG
            _userSettings.RememberMe = true;
            _userSettings.UserName = ConfigurationManager.AppSettings["hockeystreams.userName"];
            using (var secureString = ConfigurationManager.AppSettings["hockeystreams.password"].ToSecureString())
            {
                _userSettings.EncryptedPassword = secureString.EncryptString();
            }                            
#endif 

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
            HandleLoginSuccessfulMessage(new LoginSuccessfulMessage { AuthenticationResult = result });
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