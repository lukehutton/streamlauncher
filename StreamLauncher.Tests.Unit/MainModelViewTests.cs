using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Api;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.ViewModel;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Tests.Unit
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

                ViewModel = new MainViewModel(UserSettings, UserSettingsValidator, AuthenticationService, TokenProvider,
                    ViewModelLocator, DialogService, MessengerService);
            }
        }

        [TestFixture, RequiresSTA]        
        public class WhenHandleLoginSuccessfulMessageAndFirstTimeRun : GivenAMainViewModel
        {
            private ISettingsViewModel _settingsViewModel;

            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.IsFirstRun).Return(true);

                _settingsViewModel = MockRepository.GenerateMock<ISettingsViewModel>();                
                ViewModelLocator.Expect(x => x.Settings).Return(_settingsViewModel);

                var messengerService = new MessengerService();
                messengerService.Send(new LoginSuccessfulMessage()
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

        [TestFixture, RequiresSTA]        
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

                var messengerService = new MessengerService();
                messengerService.Send(new LoginSuccessfulMessage()
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
    }
}