using System;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Api;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.StartUp;
using StreamLauncher.Wpf.ViewModel;

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

            protected MainViewModel ViewModel;

            [TestFixtureSetUp]
            public void Given()
            {
                UserSettings = MockRepository.GenerateMock<IUserSettings>();                
                UserSettingsValidator = MockRepository.GenerateStub<IUserSettingsValidator>();
                AuthenticationService = MockRepository.GenerateStub<IAuthenticationService>();
                TokenProvider = MockRepository.GenerateStub<ITokenProvider>();                

                ViewModel = new MainViewModel(UserSettings, UserSettingsValidator, AuthenticationService, TokenProvider);
            }
        }

        [TestFixture, RequiresSTA]
        public class WhenHandleLoginSuccessfulMessageAndFirstTimeRun : GivenAMainViewModel
        {
            [SetUp]
            public void When()
            {
                BootStrapper.Start();
                SimpleIoc.Default.Register<IDialogService, FakeDialogService>();

                UserSettings.Expect(x => x.IsFirstRun).Return(true);

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

            [Test, Ignore("PENDING")]
            public void ItShouldShowSettingsDialog()
            {
                // todo mock window showdialog
            }

            [Test, Ignore("PENDING")]
            public void ItShouldSetIsFirstRunToFalse()
            {
                Assert.That(UserSettings.IsFirstRun, Is.EqualTo(false));
            }
        }
    }

    public class FakeDialogService : IDialogService
    {
        public void ShowError(string errorMessage, string title, string buttonText)
        {
            throw new NotImplementedException();
        }

        public void ShowError(Exception error, string title, string buttonText)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message, string title, string buttonText)
        {
            throw new NotImplementedException();
        }
    }
}