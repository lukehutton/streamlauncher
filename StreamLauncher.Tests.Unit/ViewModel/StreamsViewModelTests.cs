﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Filters;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Util;
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
            protected IStreamLocationRepository StreamLocationRepository;
            protected IUserSettings UserSettings;
            protected IMessengerService MessengerService;
            protected IPeriodicTaskRunner PeriodicTaskRunner;
            protected IViewModelLocator ViewModelLocator;

            protected IStreamsViewModel ViewModel;

            [TestFixtureSetUp]
            public void Given()
            {
                UserSettings = MockRepository.GenerateMock<IUserSettings>();
                ViewModelLocator = MockRepository.GenerateMock<IViewModelLocator>();
                DialogService = MockRepository.GenerateMock<IDialogService>();
                HockeyStreamRepository = MockRepository.GenerateMock<IHockeyStreamRepository>();
                HockeyStreamFilter = MockRepository.GenerateMock<IHockeyStreamFilter>();
                StreamLocationRepository = MockRepository.GenerateMock<IStreamLocationRepository>();                
                MessengerService = MockRepository.GenerateMock<IMessengerService>();
                PeriodicTaskRunner = MockRepository.GenerateMock<IPeriodicTaskRunner>();
                ViewModel = new StreamsViewModel(
                    HockeyStreamRepository, 
                    HockeyStreamFilter,
                    StreamLocationRepository,
                    UserSettings, 
                    DialogService, 
                    ViewModelLocator, 
                    MessengerService,
                    PeriodicTaskRunner);
            }
        }

        [TestFixture]
        public class WhenHandleAuthenticationSuccessfulMessage : GivenAStreamsViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                var context = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);

                UserSettings.Expect(x => x.PreferredEventType).Return("NHL");
                UserSettings.Expect(x => x.PreferredLocation).Return("North America - East");
                UserSettings.Expect(x => x.RefreshStreamsEnabled).Return(true);
                UserSettings.Expect(x => x.RefreshStreamsIntervalInMinutes).Return(2);
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
            public void ItShouldAutoRefreshStreams()
            {
                PeriodicTaskRunner.AssertWasCalled(
                    x =>
                        x.Run(Arg<Action>.Is.Anything, Arg<TimeSpan>.Matches(y => y == TimeSpan.FromMinutes(2)),
                            Arg<CancellationToken>.Is.Anything));
            }
        }    
        
        [TestFixture]
        public class WhenGetStreamsCommandWithNoStreams : GivenAStreamsViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                var context = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);
                UserSettings.Expect(x => x.PreferredEventType).Return("NHL");
                HockeyStreamRepository.Expect(x => x.GetLiveStreams(Arg<DateTime>.Is.Anything))
                    .Return(Task.FromResult<IEnumerable<HockeyStream>>(new List<HockeyStream>()));

                var task = ViewModel.GetStreamsCommand.ExecuteAsync();
                task.Wait();
            }          

            [Test]
            public void ItShouldSendBusyMessageToGetStreams()
            {
                MessengerService.AssertWasCalled(
                    x =>
                        x.Send(Arg<BusyStatusMessage>.Matches(y => y.IsBusy && y.Status == "Getting streams..."),
                            Arg<Guid>.Matches(y => y == MessengerTokens.MainViewModelToken)));
            }

            [Test]
            public void ItShouldGetStreams()
            {
                HockeyStreamRepository.AssertWasCalled(x => x.GetLiveStreams(Arg<DateTime>.Is.Anything));
            }
        }

        [TestFixture]
        public class WhenGetStreamsCommandWithSomeStreams : GivenAStreamsViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                var context = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);
                ViewModel.SelectedFilterEventType = "NHL";
                ViewModel.FavouriteTeam = "Favourite";
                HockeyStreamRepository.Expect(x => x.GetLiveStreams(Arg<DateTime>.Is.Anything))
                    .Return(Task.FromResult<IEnumerable<HockeyStream>>(new List<HockeyStream>()
                    {
                        new HockeyStream()
                        {
                            HomeTeam = ViewModel.FavouriteTeam
                        }
                    }));
                HockeyStreamFilter.Expect(
                    x => x.By(Arg<IList<HockeyStream>>.Is.Anything, Arg<EventTypeFilterSpecification>.Is.Anything))
                    .Return(
                        new List<HockeyStream>());

                var task = ViewModel.GetStreamsCommand.ExecuteAsync();
                task.Wait();
            }          

            [Test]
            public void ItShouldSendBusyMessageToGetStreams()
            {
                MessengerService.AssertWasCalled(
                    x =>
                        x.Send(Arg<BusyStatusMessage>.Matches(y => y.IsBusy && y.Status == "Getting streams..."),
                            Arg<Guid>.Matches(y => y == MessengerTokens.MainViewModelToken)));
            }

            [Test]
            public void ItShouldGetStreams()
            {
                HockeyStreamRepository.AssertWasCalled(x => x.GetLiveStreams(Arg<DateTime>.Is.Anything));
            }

            [Test]
            public void ItShouldFilter()
            {
                HockeyStreamFilter.AssertWasCalled(
                    x => x.By(Arg<IList<HockeyStream>>.Is.Anything, Arg<EventTypeFilterSpecification>.Is.Anything),
                    options => options.Repeat.Twice());
            }

            [Test]
            public void ItShouldMarkFavouriteTeam()
            {
                Assert.That(ViewModel.AllHockeyStreams.First().HomeTeam, Is.EqualTo("*" + ViewModel.FavouriteTeam));
                Assert.That(ViewModel.AllHockeyStreams.First().IsFavorite, Is.True);
            }
        }             
        
        [TestFixture]
        public class WhenHandleSettingsUpdatedMessage : GivenAStreamsViewModel
        {
            [TestFixtureSetUp]
            public void When()
            {
                var context = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);

                UserSettings.Expect(x => x.RefreshStreamsEnabled).Return(true);
                UserSettings.Expect(x => x.RefreshStreamsIntervalInMinutes).Return(2);

                ViewModel.HandleUserSettingsUpdatedMessage(new UserSettingsUpdated());
            }

            [Test]
            public void ItShouldAutoRefreshStreams()
            {
                PeriodicTaskRunner.AssertWasCalled(
                    x =>
                        x.Run(Arg<Action>.Is.Anything, Arg<TimeSpan>.Matches(y => y == TimeSpan.FromMinutes(2)),
                            Arg<CancellationToken>.Is.Anything));
            }
        }       
    }
}