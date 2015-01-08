using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Api;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Messages;
using StreamLauncher.Repositories;
using StreamLauncher.Security;
using StreamLauncher.Services;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Wpf.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IUserSettings _userSettings;
        private readonly IUserSettingsValidator _userSettingsValidator;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILiveStreamer _liveStreamer;

        private string _userName;
        private string _currentUser;
        private string _currentDate;
        
        public RelayCommand LogoutCommand { get; private set; }

        public RelayCommand<CancelEventArgs> Closing { get; private set; }

        public MainViewModel(IUserSettings userSettings,
            IUserSettingsValidator userSettingsValidator,
            IAuthenticationService authenticationService,
            ITokenProvider tokenProvider,
            ILiveStreamer liveStreamer)
        {
            _userSettings = userSettings;
            _userSettingsValidator = userSettingsValidator;
            _authenticationService = authenticationService;
            _tokenProvider = tokenProvider;
            _liveStreamer = liveStreamer;
            
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

            if (!_userSettings.IsFirstRun) return;

            _userSettings.LiveStreamerPath = Environment.Is64BitOperatingSystem
                ? LiveStreamer.Default64BitLocation
                : LiveStreamer.Default32BitLocation;

            _userSettings.MediaPlayerPath = Environment.Is64BitOperatingSystem
                ? Vlc.Default64BitLocation
                : Vlc.Default32BitLocation;

            _userSettings.MediaPlayerArguments = Vlc.DefaultArgs;

            var brokenRules =_userSettingsValidator.BrokenRules(_userSettings).ToList();
            if (brokenRules.Any())
            {
                ShowSettingsDialog(brokenRules.First());
                return;
            }
            _userSettings.IsFirstRun = false;
            _userSettings.Save();
            _liveStreamer.SaveConfig();
        }

        private void ShowSettingsDialog(string errorMessage)
        {
            var settingsViewModel = SimpleIoc.Default.GetInstance<SettingsViewModel>(Guid.NewGuid().ToString());
            settingsViewModel.ErrorMessage = errorMessage;
            settingsViewModel.LiveStreamerPath = _userSettings.LiveStreamerPath;
            settingsViewModel.MediaPlayerPath = _userSettings.MediaPlayerPath;
            settingsViewModel.MediaPlayerArguments = _userSettings.MediaPlayerArguments;
            var settingsWindow = new SettingsWindow
            {
                DataContext = settingsViewModel
            };            
            settingsWindow.ShowDialog();
        }

        private void HandleClosingCommand(CancelEventArgs obj)
        {
            _userSettings.Save();
            Application.Current.Shutdown();
        }
        
        private static void OpenLoginDialog(string userName = "", string errorMessage = "")
        {
            var loginViewModel = SimpleIoc.Default.GetInstance<LoginViewModel>(Guid.NewGuid().ToString());
            loginViewModel.UserName = userName;
            loginViewModel.ErrorMessage = errorMessage;
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
                OpenLoginDialog();                
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
                OpenLoginDialog(_userSettings.UserName, result.ErrorMessage);                
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