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
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IUserSettings _userSettings;
        private readonly IAuthenticationService _authenticationService;

        public RelayCommand OpenLoginDialogCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IUserSettings userSettings, IAuthenticationService authenticationService)
        {
            _userSettings = userSettings;
            _authenticationService = authenticationService;

            OpenLoginDialogCommand = new RelayCommand(OpenLoginWindow);

            AuthenticateUser();
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
                //todo shutdown app?
                MessageBox.Show("todo shutdown");
            }
        }

        private void AuthenticateUser()
        {
            if (IsInDesignModeStatic) return;

            if (!_userSettings.RememberMe)
            {
                OpenLoginDialogCommand.Execute(null);
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
                // todo send error message, username and password
                OpenLoginDialogCommand.Execute(null);
            }
            Messenger.Default.Send(new AuthenticatedMessage
            {
                AuthenticationResult = result
            });
        }

    }
}