using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Authentication;
using StreamLauncher.Repositories;
using StreamLauncher.Security;
using StreamLauncher.Wpf.Messages;

namespace StreamLauncher.Wpf.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserSettings _userSettings;

        private string _userName;        
        private string _errorMessage;
        private bool? _dialogResult;
        private bool _rememberMe;

        public RelayCommand<object> LoginCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public LoginViewModel(IAuthenticationService authenticationService, IUserSettings userSettings)
        {
            _authenticationService = authenticationService;
            _userSettings = userSettings;

            LoginCommand = new RelayCommand<object>(HandleLoginCommand);
            CancelCommand = new RelayCommand(HandleCancelCommand);

            Messenger.Default.Register<AutoLoginFailedMessage>(this, HandleAutoLoginFailedMessage);            
        }

        private void HandleAutoLoginFailedMessage(AutoLoginFailedMessage autoLoginFailedMessage)
        {
            UserName = autoLoginFailedMessage.UserName;
            ErrorMessage = autoLoginFailedMessage.ErrorMessage;
        }

        private void HandleCancelCommand()
        {                        
            DialogResult = false;
        }

        private void HandleLoginCommand(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;

            var result = _authenticationService.Authenticate(UserName, password);
            if (!result.IsAuthenticated)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            if (RememberMe)
            {
                _userSettings.UserName = UserName;
                using (var secureString = password.ToSecureString())
                {
                    _userSettings.EncryptedPassword = secureString.EncryptString();
                }                
                _userSettings.RememberMe = true;
            }

            Messenger.Default.Send(new LoginSuccessfulMessage
            {
                AuthenticationResult = result                
            });

            DialogResult = true;
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }        
        
        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public bool RememberMe
        {
            get
            {
                return _rememberMe;
            }

            set
            {
                _rememberMe = value;
                RaisePropertyChanged(() => RememberMe);
            }
        }

        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }

            set
            {
                _dialogResult = value;
                RaisePropertyChanged(() => DialogResult);
            }
        }
    }
}