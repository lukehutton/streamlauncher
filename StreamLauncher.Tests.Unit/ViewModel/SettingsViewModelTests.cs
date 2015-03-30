using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Util;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Tests.Unit.ViewModel
{
    [TestFixture]
    public class SettingsViewModelTests
    {
        public class GivenASettingsViewModel
        {
            protected IEnvironmentHelper EnvironmentHelper;
            protected ILiveStreamer LiveStreamer;
            protected IStreamLocationRepository StreamLocationRepository;
            protected IUserSettings UserSettings;
            protected IUserSettingsValidator UserSettingsValidator;
            protected IThreadSleeper ThreadSleeper;
            protected IMessengerService MessengerService;
            protected ISettingsViewModel ViewModel;

            [TestFixtureSetUp]
            public void Given()
            {
                UserSettings = MockRepository.GenerateMock<IUserSettings>();
                LiveStreamer = MockRepository.GenerateStub<ILiveStreamer>();
                UserSettingsValidator = MockRepository.GenerateStub<IUserSettingsValidator>();
                StreamLocationRepository = MockRepository.GenerateStub<IStreamLocationRepository>();
                EnvironmentHelper = MockRepository.GenerateStub<IEnvironmentHelper>();
                ThreadSleeper = MockRepository.GenerateStub<IThreadSleeper>();
                MessengerService = MockRepository.GenerateStub<IMessengerService>();

                ViewModel = new SettingsViewModel(
                    UserSettings,
                    LiveStreamer, 
                    UserSettingsValidator,
                    StreamLocationRepository,
                    EnvironmentHelper, 
                    ThreadSleeper,
                    MessengerService);
            }
        }

        [TestFixture]
        public class WhenInitializeSettingsForFirstTime : GivenASettingsViewModel
        {
            [SetUp]
            public void When()
            {
                StreamLocationRepository.Expect(x => x.GetLocations()).Return(Enumerable.Empty<StreamLocation>());
                EnvironmentHelper.Expect(x => x.Is64BitEnvironment()).Return(true);
                UserSettings.Expect(x => x.LiveStreamerPath).Return(string.Empty);
                UserSettings.Expect(x => x.MediaPlayerPath).Return(string.Empty);
                UserSettings.Expect(x => x.MediaPlayerArguments).Return(null);
                UserSettings.Expect(x => x.PreferredEventType).Return(string.Empty);
                UserSettings.Expect(x => x.PreferredLocation).Return(string.Empty);
                ViewModel.Init();
            }

            [Test]
            public void ItShouldGetLocations()
            {
                StreamLocationRepository.AssertWasCalled(x => x.GetLocations());
            }

            [Test]
            public void ItShouldDefaultLiveStreamerPath()
            {
                Assert.That(ViewModel.LiveStreamerPath, Is.EqualTo(MediaPlayers.LiveStreamer.Default64BitLocation));
            }

            [Test]
            public void ItShouldDefaultMediaPlayerPathToVlc()
            {
                Assert.That(ViewModel.MediaPlayerPath, Is.EqualTo(Vlc.Default64BitLocation));
            }

            [Test]
            public void ItShouldDefaultMediaPlayerArgs()
            {
                Assert.That(ViewModel.MediaPlayerArguments, Is.EqualTo(Vlc.DefaultArgs));
            }

            [Test]
            public void ItShouldDefaultPreferredEventTypetoNHL()
            {
                Assert.That(ViewModel.PreferredEventType, Is.EqualTo("NHL"));
            }

            [Test]
            public void ItShouldDefaultPreferredLocationtoNorthAmericaWest()
            {
                Assert.That(ViewModel.PreferredLocation, Is.EqualTo("North America - West"));
            }
        }

        [TestFixture]
        public class WhenSaveWithError : GivenASettingsViewModel
        {
            private const string ErrorMessage = "Oopsy, you have an error";

            [SetUp]
            public void When()
            {                
                UserSettingsValidator.Expect(x => x.BrokenRules(UserSettings)).Return(new List<string>
                {
                    ErrorMessage
                });
                ViewModel.SaveCommand.Execute(null);
            }

            [Test]
            public void ItShouldSetFirstErrorInMessage()
            {
                Assert.That(ViewModel.ErrorMessage, Is.EqualTo(ErrorMessage));
            }
        }

        [TestFixture]
        public class WhenSave : GivenASettingsViewModel
        {            
            [SetUp]
            public void When()
            {
                UserSettingsValidator.Expect(x => x.BrokenRules(UserSettings)).Return(new List<string>());                
                var task = ViewModel.SaveCommand.ExecuteAsync(); 
                task.Wait();
            }

            [Test]
            public void ItShouldSaveSettings()
            {
                UserSettings.AssertWasCalled(x => x.Save());
            }

            [Test]
            public void ItShouldSaveLiveStreamerConfig()
            {
                LiveStreamer.AssertWasCalled(x => x.SaveConfig());
            }
        }

        [TestFixture]
        public class WhenCancel: GivenASettingsViewModel
        {            
            [SetUp]
            public void When()
            {                                
                ViewModel.CancelCommand.Execute(null);
            }

            [Test]
            public void ItShouldCloseDialog()
            {
                Assert.That(ViewModel.DialogResult, Is.EqualTo(false));
            }
        }
    }
}