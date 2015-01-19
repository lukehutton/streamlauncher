using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.Messages;
using StreamLauncher.Repositories;
using StreamLauncher.Security;
using StreamLauncher.Services;
using StreamLauncher.Util;
using StreamLauncher.Wpf.Infrastructure;

namespace StreamLauncher.Wpf.ViewModel
{
    public class LoginViewModel : ViewModelBase, ILoginViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserSettings _userSettings;
        private readonly IMessengerService _messengerService;

        private string _busyText;
        private bool _isBusy;

        private string _userName;        
        private string _errorMessage;
        private bool? _dialogResult;
        private bool _rememberMe;

        public AsyncRelayCommand<object> LoginCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public LoginViewModel(IAuthenticationService authenticationService, IUserSettings userSettings, IMessengerService messengerService)
        {
            _authenticationService = authenticationService;
            _userSettings = userSettings;
            _messengerService = messengerService;

            LoginCommand = new AsyncRelayCommand<object>(HandleLoginCommand);
            CancelCommand = new RelayCommand(HandleCancelCommand);            
        }

        private void HandleCancelCommand()
        {                        
            DialogResult = false;
        }

        private async Task AuthenticateAsync(string userName, string password)
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
                _userSettings.Save();
            }

            _messengerService.Send(new LoginSuccessfulMessage
            {
                AuthenticationResult = result
            });
            
            DialogResult = true;             
        }

        private async Task HandleLoginCommand(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;

            if (UserName.IsNullOrEmpty())
            {
                ErrorMessage = "User Name must not be empty.";
                return;
            }

            if (password.IsNullOrEmpty())
            {
                ErrorMessage = "Password must not be empty.";
                return;
            }

            await AuthenticateAsync(UserName, password);                      
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
                RaisePropertyChanged(() => BusyText);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }     
    }
}