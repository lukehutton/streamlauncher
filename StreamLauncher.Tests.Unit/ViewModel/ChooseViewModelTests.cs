using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Tests.Unit.ViewModel
{
    [TestFixture]
    public class ChooseFeedsViewModelTests
    {
        public class GivenAChooseFeedsViewModel
        {
            protected IDialogService DialogService;            
            protected IHockeyStreamRepository HockeyStreamRepository;
            protected ILiveStreamer LiveStreamer;
            protected IViewModelLocator ViewModelLocator;

            protected IChooseFeedsViewModel ViewModel;

            [TestFixtureSetUp]
            public void Given()
            {                
                ViewModelLocator = MockRepository.GenerateMock<IViewModelLocator>();
                DialogService = MockRepository.GenerateMock<IDialogService>();
                HockeyStreamRepository = MockRepository.GenerateMock<IHockeyStreamRepository>();                                
                LiveStreamer = MockRepository.GenerateMock<ILiveStreamer>();                
                ViewModel = new ChooseFeedsViewModel(HockeyStreamRepository, LiveStreamer, DialogService, ViewModelLocator);
            }
        }
    
        [TestFixture]
        public class WhenPlayCommand : GivenAChooseFeedsViewModel
        {
            private const int StreamId = 2342;

            [TestFixtureSetUp]
            public void When()
            {
                var context = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);

                var feeds = new List<Feed>
                {
                    new Feed
                    {
                        FeedType = "NBC Feed",
                        Game = "Team 1 at Team 2",
                        IsPlaying = true,
                        StreamId = StreamId
                    }
                };

                ViewModel.Init(feeds, "West", Quality.HD);

                HockeyStreamRepository.Expect(x => x.GetLiveStream(StreamId, "West", Quality.HD)).Return(Task.FromResult(new LiveStream
                {
                    Source = @"RTMP:\\somewhere"
                }));

                var task = ViewModel.PlayCommand.ExecuteAsync(StreamId);
                task.Wait();
            }

            [Test]
            public void ItShouldGetLiveStreamWithSelectedStreamIdAndLocationAndQuality()
            {
                HockeyStreamRepository.AssertWasCalled(x => x.GetLiveStream(StreamId, "West", Quality.HD));
            }

            [Test]
            public void ItShouldPlayStreamSource()
            {
                LiveStreamer.AssertWasCalled(x => x.Play("Team 1 at Team 2", @"RTMP:\\somewhere", Quality.HD));
            }
        }
    }
}