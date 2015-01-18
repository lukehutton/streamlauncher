using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Api;
using StreamLauncher.Messages;
using StreamLauncher.Repositories;
using StreamLauncher.Security;
using StreamLauncher.Services;
using StreamLauncher.Util;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Wpf.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly IUserSettings _userSettings;
        private readonly IUserSettingsValidator _userSettingsValidator;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IViewModelLocator _viewModelLocator;
        private readonly IDialogService _dialogService;
        private readonly IMessengerService _messengerService;
        private readonly IApplicationDispatcher _applicationDispatcher;

        private string _busyText;
        private bool _isBusy;

        private string _userName;
        private string _currentUser;
        private string _currentDate;
        private string _title;

        public RelayCommand LogoutCommand { get; private set; }

        public RelayCommand<CancelEventArgs> Closing { get; private set; }

        public MainViewModel(
            IUserSettings userSettings,
            IUserSettingsValidator userSettingsValidator,
            IAuthenticationService authenticationService,
            ITokenProvider tokenProvider,
            IViewModelLocator viewModelLocator,
            IDialogService dialogService,
            IMessengerService messengerService,
            IApplicationDispatcher applicationDispatcher)
        {
            _userSettings = userSettings;
            _userSettingsValidator = userSettingsValidator;
            _authenticationService = authenticationService;
            _tokenProvider = tokenProvider;
            _viewModelLocator = viewModelLocator;
            _dialogService = dialogService;
            _messengerService = messengerService;
            _applicationDispatcher = applicationDispatcher;

            LogoutCommand = new RelayCommand(HandleLogoutCommand);
            Closing = new RelayCommand<CancelEventArgs>(HandleClosingCommand);

            Messenger.Default.Register<LoginSuccessfulMessage>(this, HandleLoginSuccessfulMessage);
            Messenger.Default.Register<BusyStatusMessage>(this, HandleBusyStatusMessage);
            Messenger.Default.Register<AuthenticateMessage>(this, HandleAuthenticateMessage);
        }

        public async void HandleAuthenticateMessage(AuthenticateMessage authenticateMessage)
        {
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
            var result = await _authenticationService.Authenticate(_userSettings.UserName, password);
            if (!result.IsAuthenticated)
            {
                OpenLoginDialog(_userSettings.UserName, result.ErrorMessage);
                return;
            }

            HandleLoginSuccessfulMessage(new LoginSuccessfulMessage { AuthenticationResult = result });
        }

        public void HandleBusyStatusMessage(BusyStatusMessage busyStatusMessage)
        {            
            BusyText = busyStatusMessage.Status;
            IsBusy = busyStatusMessage.IsBusy;
        }

        public void HandleLoginSuccessfulMessage(LoginSuccessfulMessage loginSuccessful)
        {
            Title = "Hockey Streams Launcher v" + GetPublishedVersion();

            _tokenProvider.Token = loginSuccessful.AuthenticationResult.AuthenticatedUser.Token;
            _userName = loginSuccessful.AuthenticationResult.AuthenticatedUser.UserName;

            CurrentUser = string.Format("Hi {0}", _userName);
            CurrentDate = DateTime.Now.ToString("dddd, MMMM dd");

            if (_userSettings.IsFirstRun)
            {
                ShowSettingsDialog();
                _userSettings.IsFirstRun = false;
            }
            else
            {

                var brokenRules = _userSettingsValidator.BrokenRules(_userSettings).ToList();
                if (brokenRules.Any())
                {
                    ShowSettingsDialog(brokenRules.First());
                }
            }

            _messengerService.Send(new AuthenticatedMessage
            {
                AuthenticationResult = loginSuccessful.AuthenticationResult
            });
        }

        private string GetPublishedVersion()
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.
                    CurrentVersion.ToString();
            }
            return "0.0.0.0";
        }

        private void ShowSettingsDialog(string errorMessage = "")
        {
            var settingsViewModel = _viewModelLocator.Settings;
            settingsViewModel.Init();
            settingsViewModel.ErrorMessage = errorMessage;
            _dialogService.ShowDialog<SettingsWindow>(settingsViewModel);
        }

        private void HandleClosingCommand(CancelEventArgs obj)
        {
            _userSettings.Save();
            _applicationDispatcher.Shutdown();
        }
        
        private void OpenLoginDialog(string userName = "", string errorMessage = "")
        {
            var loginViewModel = _viewModelLocator.Login;
            loginViewModel.UserName = userName;
            loginViewModel.ErrorMessage = errorMessage;
            var authenticated = _dialogService.ShowDialog<LoginWindow>(loginViewModel) ?? false;
            if (!authenticated)
            {
                _applicationDispatcher.Shutdown();
            }            
        }

        private void HandleLogoutCommand()
        {
            _messengerService.Send(new NotificationMessage(this, "HideMainWindow"));

            _tokenProvider.Token = string.Empty;

            _userSettings.UserName = string.Empty;
            _userSettings.EncryptedPassword = string.Empty;
            _userSettings.RememberMe = false;
            _userSettings.Save();            

            OpenLoginDialog();

            _messengerService.Send(new NotificationMessage(this, "ShowMainWindow"));
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
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
    }
}