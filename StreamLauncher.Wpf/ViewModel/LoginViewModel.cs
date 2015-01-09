using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Messages;
using StreamLauncher.Repositories;
using StreamLauncher.Security;
using StreamLauncher.Services;
using StreamLauncher.Util;

namespace StreamLauncher.Wpf.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserSettings _userSettings;

        private string _busyText;
        private bool _isBusy;

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
        }

        private void HandleCancelCommand()
        {                        
            DialogResult = false;
        }

        private async void AuthenticateAsync(string userName, string password)
        {
            BusyText = "Logging in...";
            IsBusy = true;

            var result = await Task.Run(() => _authenticationService.Authenticate(userName, password));                            
            IsBusy = false;

            if (!result.IsAuthenticated)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            if (RememberMe)
            {
                _userSettings.UserName = userName;
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

        private void HandleLoginCommand(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;

            if (UserName.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                ErrorMessage = "User Name or Password must not be empty.";
                return;
            }

            AuthenticateAsync(UserName, password);                      
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