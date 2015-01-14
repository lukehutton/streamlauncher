using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Filters;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Tests.Unit.ViewModel
{
    [TestFixture]
    public class StreamsViewModelTests
    {
        public class GivenAStreamsViewModel
        {
            protected IDialogService DialogService;
            protected IHockeyStreamFilter HockeyStreamFilter;
            protected IHockeyStreamRepository HockeyStreamRepository;
            protected ILiveStreamer LiveStreamer;
            protected IStreamLocationRepository StreamLocationRepository;
            protected IUserSettings UserSettings;
            protected StreamsViewModel ViewModel;
            protected IViewModelLocator ViewModelLocator;

            [TestFixtureSetUp]
            public void Given()
            {
                UserSettings = MockRepository.GenerateMock<IUserSettings>();
                ViewModelLocator = MockRepository.GenerateMock<IViewModelLocator>();
                DialogService = MockRepository.GenerateMock<IDialogService>();
                HockeyStreamRepository = MockRepository.GenerateMock<IHockeyStreamRepository>();
                HockeyStreamFilter = MockRepository.GenerateMock<IHockeyStreamFilter>();
                StreamLocationRepository = MockRepository.GenerateMock<IStreamLocationRepository>();
                LiveStreamer = MockRepository.GenerateMock<ILiveStreamer>();
                ViewModel = new StreamsViewModel(HockeyStreamRepository, HockeyStreamFilter, StreamLocationRepository,
                    LiveStreamer, UserSettings, DialogService, ViewModelLocator);
            }
        }
    }
}