using System.Threading.Tasks;
using System.Windows.Controls;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Tests.Unit.ViewModel
{
    [TestFixture]
    public class LoginViewModelTests
    {
        public class GivenALoginViewModel
        {
            protected IAuthenticationService AuthenticationService;
            protected IMessengerService MessengerService;
            protected IUserSettings UserSettings;
            protected LoginViewModel ViewModel;

            [TestFixtureSetUp]
            public void Given()
            {
                AuthenticationService = MockRepository.GenerateStub<IAuthenticationService>();
                UserSettings = MockRepository.GenerateMock<IUserSettings>();
                MessengerService = MockRepository.GenerateStub<IMessengerService>();

                ViewModel = new LoginViewModel(AuthenticationService, UserSettings, MessengerService);
            }
        }

        [TestFixture, RequiresSTA]
        public class WhenHandleLoginWithEmptyUserName : GivenALoginViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                ViewModel.UserName = string.Empty;
                var passwordBox = new PasswordBox {Password = "password"};
                ViewModel.LoginCommand.Execute(passwordBox);
            }

            [Test]
            public void ItShouldSetError()
            {
                Assert.That(ViewModel.ErrorMessage, Is.EqualTo("User Name must not be empty."));
            }
            
            [Test]
            public void ItShouldNotAttemptToAuthenticate()
            {
                AuthenticationService.AssertWasNotCalled(x => x.Authenticate(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            }
        }
        
        [TestFixture, RequiresSTA]
        public class WhenHandleLoginWithEmptyPassword : GivenALoginViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                ViewModel.UserName = "User Name";
                var passwordBox = new PasswordBox {Password = ""};
                ViewModel.LoginCommand.Execute(passwordBox);
            }

            [Test]
            public void ItShouldSetError()
            {
                Assert.That(ViewModel.ErrorMessage, Is.EqualTo("Password must not be empty."));
            }
            
            [Test]
            public void ItShouldNotAttemptToAuthenticate()
            {
                AuthenticationService.AssertWasNotCalled(x => x.Authenticate(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            }
        }       
        
        [TestFixture, RequiresSTA]
        public class WhenHandleLoginWithInvalidAuth : GivenALoginViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                AuthenticationService.Expect(x => x.Authenticate("User Name", "password"))
                    .Return(Task.FromResult(new AuthenticationResult
                    {
                        IsAuthenticated = false,
                        ErrorMessage = "Bad login."
                    }));

                ViewModel.UserName = "User Name";
                var passwordBox = new PasswordBox {Password = "password"};
                ViewModel.LoginCommand.Execute(passwordBox);
            }

            [Test, Ignore("PENDING")]
            public void ItShouldSetError()
            {                
                Assert.That(ViewModel.ErrorMessage, Is.EqualTo("Bad login."));
            }
        }
    }
}