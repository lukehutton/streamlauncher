using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Util;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Tests.Unit
{
    public class GivenASettingsViewModel
    {
        protected ILiveStreamer LiveStreamer;
        protected IStreamLocationRepository StreamLocationRepository;
        protected IUserSettings UserSettings;
        protected IUserSettingsValidator UserSettingsValidator;
        protected IEnvironmentHelper EnvironmentHelper;
        protected SettingsViewModel ViewModel;

        [TestFixtureSetUp]
        public void Given()
        {
            UserSettings = MockRepository.GenerateMock<IUserSettings>();
            LiveStreamer = MockRepository.GenerateStub<ILiveStreamer>();
            UserSettingsValidator = MockRepository.GenerateStub<IUserSettingsValidator>();
            StreamLocationRepository = MockRepository.GenerateStub<IStreamLocationRepository>();
            EnvironmentHelper = MockRepository.GenerateStub<IEnvironmentHelper>();

            ViewModel = new SettingsViewModel(UserSettings, LiveStreamer, UserSettingsValidator,
                StreamLocationRepository, EnvironmentHelper);
        }
    }

    [TestFixture]
    public class WhenInitializeSettingsFirstTime : GivenASettingsViewModel
    {
        [SetUp]
        public void When()
        {
            StreamLocationRepository.Expect(x => x.GetLocations()).Return(Enumerable.Empty<StreamLocation>());
            EnvironmentHelper.Expect(x => x.Is64BitEnvironment()).Return(true);
            UserSettings.Expect(x => x.LiveStreamerPath).Return(string.Empty);
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
    }
}