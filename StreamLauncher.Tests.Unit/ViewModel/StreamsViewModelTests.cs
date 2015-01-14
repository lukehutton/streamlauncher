using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Filters;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Messages;
using StreamLauncher.Models;
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
            protected IMessengerService MessengerService;
            protected IViewModelLocator ViewModelLocator;

            protected StreamsViewModel ViewModel;

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
                MessengerService = MockRepository.GenerateMock<IMessengerService>();
                ViewModel = new StreamsViewModel(HockeyStreamRepository, HockeyStreamFilter, StreamLocationRepository,
                    LiveStreamer, UserSettings, DialogService, ViewModelLocator, MessengerService);
            }
        }

        [TestFixture]
        public class WhenHandleAuthenticationSuccessfulMessage : GivenAStreamsViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                UserSettings.Expect(x => x.PreferredEventType).Return("NHL");
                UserSettings.Expect(x => x.PreferredLocation).Return("North America - East");
                StreamLocationRepository.Expect(x => x.GetLocations()).Return(new List<StreamLocation>());
                HockeyStreamRepository.Expect(x => x.GetLiveStreams(Arg<DateTime>.Is.Anything))
                    .Return(Task.FromResult<IEnumerable<HockeyStream>>(new List<HockeyStream>()));
                HockeyStreamFilter.Expect(
                    x => x.By(Arg<IList<HockeyStream>>.Is.Anything, Arg<EventTypeFilterSpecification>.Is.Anything))
                    .Return(
                        new List<HockeyStream>());
                ViewModel.HandleAuthenticationSuccessfulMessage(new AuthenticatedMessage
                {
                    AuthenticationResult = new AuthenticationResult
                    {
                        AuthenticatedUser = new User
                        {
                            FavoriteTeam = "The Destroyers"
                        }
                    }
                });
            }

            [Test]
            public void ItShouldSetPrefferedFilteredEventType()
            {
                Assert.That(ViewModel.SelectedFilterEventType, Is.EqualTo("NHL"));
            }

            [Test]
            public void ItShouldSetActiveStateToAll()
            {
                Assert.That(ViewModel.SelectedFilterActiveState, Is.EqualTo("ALL"));
            }

            [Test]
            public void ItShouldSetFavouriteTeam()
            {
                Assert.That(ViewModel.FavouriteTeam, Is.EqualTo("The Destroyers"));
            }

            [Test]
            public void ItShouldSetIsAuthenticated()
            {
                Assert.That(ViewModel.IsAuthenticated, Is.True);
            }

            [Test]
            public void ItShouldSetPrefferedLocation()
            {
                Assert.That(ViewModel.SelectedLocation, Is.EqualTo("North America - East"));
            }

            [Test]
            public void ItShouldSendBusyMessageToGetStreams()
            {
                MessengerService.AssertWasCalled(
                    x => x.Send(Arg<BusyStatusMessage>.Matches(y => y.IsBusy && y.Status == "Getting streams...")));
            }

            [Test]
            public void ItShouldGetStreams()
            {
                HockeyStreamRepository.AssertWasCalled(x => x.GetLiveStreams(Arg<DateTime>.Is.Anything));
            }

            [Test]
            public void ItShouldFilterByEventType()
            {
                HockeyStreamFilter.AssertWasCalled(
                    x => x.By(Arg<IList<HockeyStream>>.Is.Anything, Arg<EventTypeFilterSpecification>.Is.Anything),
                    options => options.Repeat.Once());
            }
        }

        [TestFixture]
        public class WhenPlayHomeFeedCommand : GivenAStreamsViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                ViewModel.SelectedStream = new HockeyStream
                {
                    HomeStreamId = 2342,
                    AwayTeam = "Canucks",
                    HomeTeam = "Maple Leafs"
                };
                ViewModel.SelectedQuality = "High Quality (3200Kbps HD)";
                ViewModel.SelectedLocation = "North America - West";
                HockeyStreamRepository.Expect(x => x.GetLiveStream(2342, "North America - West", Quality.HD)).Return(Task.FromResult(new LiveStream
                {
                    Source = @"RTMP:\\somewhere"
                }));
                ViewModel.PlayHomeFeedCommand.Execute(null);
            }

            [Test]
            public void ItShouldGetLiveStreamWithSelectedStreamIdAndLocationAndQuality()
            {
                HockeyStreamRepository.AssertWasCalled(x => x.GetLiveStream(2342, "North America - West", Quality.HD));
            }

            [Test]
            public void ItShouldPlayStreamSource()
            {
                LiveStreamer.AssertWasCalled(x => x.Play("Canucks at Maple Leafs", @"RTMP:\\somewhere", Quality.HD));
            }
        }
    }
}