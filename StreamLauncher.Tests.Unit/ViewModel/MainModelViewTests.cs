using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Api;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Util;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.ViewModel;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Tests.Unit.ViewModel
{
    [TestFixture]
    public class MainModelViewTests
    {                
        public class GivenAMainViewModel
        {
            protected IUserSettings UserSettings;
            protected IUserSettingsValidator UserSettingsValidator;
            protected IAuthenticationService AuthenticationService;
            protected ITokenProvider TokenProvider;
            protected IViewModelLocator ViewModelLocator;
            protected IDialogService DialogService;
            protected IMessengerService MessengerService;
            protected IApplicationDispatcher ApplicationDispatcher;

            protected MainViewModel ViewModel;

            [TestFixtureSetUp]
            public void Given()
            {
                UserSettings = MockRepository.GenerateMock<IUserSettings>();                
                UserSettingsValidator = MockRepository.GenerateStub<IUserSettingsValidator>();
                AuthenticationService = MockRepository.GenerateStub<IAuthenticationService>();
                TokenProvider = MockRepository.GenerateMock<ITokenProvider>();
                ViewModelLocator = MockRepository.GenerateMock<IViewModelLocator>();
                DialogService = MockRepository.GenerateMock<IDialogService>();
                MessengerService = MockRepository.GenerateMock<IMessengerService>();
                ApplicationDispatcher = MockRepository.GenerateMock<IApplicationDispatcher>();

                ViewModel = new MainViewModel(UserSettings, UserSettingsValidator, AuthenticationService, TokenProvider,
                    ViewModelLocator, DialogService, MessengerService, ApplicationDispatcher);
            }
        }

        [TestFixture]        
        public class WhenHandleLoginSuccessfulMessageAndFirstTimeRun : GivenAMainViewModel
        {
            private ISettingsViewModel _settingsViewModel;

            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.IsFirstRun).Return(true);

                _settingsViewModel = MockRepository.GenerateMock<ISettingsViewModel>();                
                ViewModelLocator.Expect(x => x.Settings).Return(_settingsViewModel);

                ViewModel.HandleLoginSuccessfulMessage(new LoginSuccessfulMessage()
                {
                    AuthenticationResult = new AuthenticationResult
                    {
                        AuthenticatedUser = new User
                        {
                            FavoriteTeam = "Vancouver Canucks",
                            Token = "Secret Token",
                            UserName = "Foo bar"
                        }
                    }
                });
            }

            [Test]
            public void ItShouldShowInitializeSettings()
            {
                _settingsViewModel.AssertWasCalled(x => x.Init());
            }

            [Test]
            public void ItShouldShowSettingsDialog()
            {                
                DialogService.AssertWasCalled(x => x.ShowDialog<SettingsWindow>(_settingsViewModel));
            }

            [Test]
            public void ItShouldSetIsFirstRunToFalse()
            {
                UserSettings.AssertWasCalled(x => x.IsFirstRun = false);
            }
            
            [Test]
            public void ItShouldSetToken()
            {
                TokenProvider.AssertWasCalled(x => x.Token = "Secret Token");
            }     
       
            [Test]
            public void ItShouldSetUserName()
            {
                Assert.That(ViewModel.CurrentUser, Is.EqualTo("Hi Foo bar"));
            }

            [Test]
            public void ItShouldSendAuthenticatedMessage()
            {
                MessengerService.AssertWasCalled(x => x.Send(Arg<AuthenticatedMessage>.Is.Anything));
            }
        }

        [TestFixture]        
        public class WhenHandleLoginSuccessfulMessageAndBrokenSettings : GivenAMainViewModel
        {
            private ISettingsViewModel _settingsViewModel;

            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.IsFirstRun).Return(false);
                UserSettingsValidator.Expect(x => x.BrokenRules(UserSettings)).Return(new List<string>
                {
                    "Oops! Bad Path"
                });

                _settingsViewModel = MockRepository.GenerateMock<ISettingsViewModel>();                
                ViewModelLocator.Expect(x => x.Settings).Return(_settingsViewModel);

                ViewModel.HandleLoginSuccessfulMessage(new LoginSuccessfulMessage()
                {
                    AuthenticationResult = new AuthenticationResult
                    {
                        AuthenticatedUser = new User
                        {
                            FavoriteTeam = "Vancouver Canucks",
                            Token = "Secret Token",
                            UserName = "Foo bar"
                        }
                    }
                });
            }

            [Test]
            public void ItShouldShowInitializeSettings()
            {
                _settingsViewModel.AssertWasCalled(x => x.Init());
            }

            [Test]
            public void ItShouldSetErrorOnSettings()
            {
                _settingsViewModel.AssertWasCalled(x => x.ErrorMessage = "Oops! Bad Path");
            }

            [Test]
            public void ItShouldShowSettingsDialog()
            {                
                DialogService.AssertWasCalled(x => x.ShowDialog<SettingsWindow>(_settingsViewModel));
            }
            
            [Test]
            public void ItShouldSetToken()
            {
                TokenProvider.AssertWasCalled(x => x.Token = "Secret Token");
            }     
       
            [Test]
            public void ItShouldSetUserName()
            {
                Assert.That(ViewModel.CurrentUser, Is.EqualTo("Hi Foo bar"));
            }

            [Test]
            public void ItShouldSendAuthenticatedMessage()
            {
                MessengerService.AssertWasCalled(x => x.Send(Arg<AuthenticatedMessage>.Is.Anything));
            }
        }

        [TestFixture]
        public class WhenHandleAuthenticateMessageAndRememberMeNotSet : GivenAMainViewModel
        {
            private ILoginViewModel _loginViewModel;

            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.RememberMe).Return(false);

                _loginViewModel = MockRepository.GenerateMock<ILoginViewModel>();
                ViewModelLocator.Expect(x => x.Login).Return(_loginViewModel);

                var messengerService = new MessengerService();
                messengerService.Send(new AuthenticateMessage());
            }

            [Test]
            public void ItShouldShowLoginDialog()
            {
                DialogService.AssertWasCalled(x => x.ShowDialog<LoginWindow>(_loginViewModel));                
            }
        }
        
        [TestFixture, RequiresSTA]
        public class WhenHandleAuthenticateMessageAndRememberMeSetAndUserGood : GivenAMainViewModel
        {
            private ILoginViewModel _loginViewModel;

            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.RememberMe).Return(true);

                _loginViewModel = MockRepository.GenerateMock<ILoginViewModel>();
                ViewModelLocator.Expect(x => x.Login).Return(_loginViewModel);

                AuthenticationService.Expect(x => x.Authenticate(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                    .Return(Task.FromResult(new AuthenticationResult
                    {
                        IsAuthenticated = true,
                        AuthenticatedUser = new User
                        {
                            Token = "Secret Token"
                        }
                    }));

                ViewModel.HandleAuthenticateMessage(new AuthenticateMessage());                
            }

            [Test]
            public void ItShouldSetToken()
            {
                TokenProvider.AssertWasCalled(x => x.Token = "Secret Token");
            }
        }

        [TestFixture]
        public class WhenHandleAuthenticateMessageAndRememberMeSetAndUserBad : GivenAMainViewModel
        {
            private ILoginViewModel _loginViewModel;

            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.RememberMe).Return(true);

                _loginViewModel = MockRepository.GenerateMock<ILoginViewModel>();
                ViewModelLocator.Expect(x => x.Login).Return(_loginViewModel);

                AuthenticationService.Expect(x => x.Authenticate(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                    .Return(Task.FromResult(new AuthenticationResult
                    {
                        IsAuthenticated = false
                    }));
                ViewModel.HandleAuthenticateMessage(new AuthenticateMessage());     
            }

            [Test]
            public void ItShouldShowLoginDialog()
            {
                DialogService.AssertWasCalled(x => x.ShowDialog<LoginWindow>(_loginViewModel));
            }
        }

        [TestFixture]
        public class WhenLogout : GivenAMainViewModel
        {
            private ILoginViewModel _loginViewModel;

            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.RememberMe).Return(true);

                _loginViewModel = MockRepository.GenerateMock<ILoginViewModel>();
                ViewModelLocator.Expect(x => x.Login).Return(_loginViewModel);

                ViewModel.LogoutCommand.Execute(null);
            }

            [Test]
            public void ItShouldClearSettingsAndSave()
            {
                UserSettings.AssertWasCalled(x => x.UserName = string.Empty);
                UserSettings.AssertWasCalled(x => x.Save());
            }

            [Test]
            public void ItShouldShowLoginDialog()
            {
                DialogService.AssertWasCalled(x => x.ShowDialog<LoginWindow>(_loginViewModel));
            }
        }
    }
}