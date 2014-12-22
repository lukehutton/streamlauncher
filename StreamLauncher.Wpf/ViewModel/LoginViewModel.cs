using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.Authentication;

namespace StreamLauncher.Wpf.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;

        private string _userName;
        private string _password;
        private string _errorMessage;

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public LoginViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            LoginCommand = new RelayCommand(Login);
            CancelCommand = new RelayCommand(Cancel);            
        }

        private void Cancel()
        {            
        }

        private void Login()
        {                     
            var result = _authenticationService.Authenticate(UserName, Password);
            if (!result.IsAuthenticated)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }
           
//            _tokenProvider.Token = result.AuthenticatedUser.Token;
//            _authenticatedUser = result.AuthenticatedUser;
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                if (_errorMessage == value)
                {
                    return;
                }

                _errorMessage = value;
                RaisePropertyChanged(() => UserName);
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
                if (_userName == value)
                {
                    return;
                }

                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (_password == value)
                {
                    return;
                }

                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }
    }
}