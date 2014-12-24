using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Authentication;
using StreamLauncher.Wpf.Messages;

namespace StreamLauncher.Wpf.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;

        private string _userName;        
        private string _errorMessage;
        private bool? _dialogResult;
        private bool _rememberMe;

        public RelayCommand<object> LoginCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public LoginViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            LoginCommand = new RelayCommand<object>(Login);
            CancelCommand = new RelayCommand(Cancel);            
        }

        private void Cancel()
        {                        
            DialogResult = false;
        }

        private void Login(object parameter)
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
                //todo if remember me set save credentials                
            }

            Messenger.Default.Send(new AuthenticatedMessage
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