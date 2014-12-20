using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Dtos;
using StreamLauncher.Models;
using StreamLauncher.Providers;

namespace StreamLauncher.Tests.Unit
{
    public class LiveStreamScheduleAggregatorAndMapperTests
    {
        public class GivenAListOfFeedsAndAggregator
        {
            protected ILiveStreamScheduleAggregatorAndMapper LiveStreamScheduleAggregatorAndMapper;
            protected List<LiveStreamDto> LiveStreams;

            [TestFixtureSetUp]
            public void Given()
            {
                LiveStreams = BuildLiveStreams();
                LiveStreamScheduleAggregatorAndMapper = new LiveStreamScheduleAggregatorAndMapper();
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
                    new LiveStreamDto
                    {
                        Id = "34173",
                        AwayTeam = "Tampa Bay Lightning",
                        HomeTeam = "New Jersey Devils",
                        FeedType = "Away Feed"
                    },
                    new LiveStreamDto
                    {
                        Id = "34172",
                        AwayTeam = "Tampa Bay Lightning",
                        HomeTeam = "New Jersey Devils",
                        FeedType = "Home Feed"
                    }
                };
            }
        }

        [TestFixture]
        public class WhenAggregateAndMap : GivenAListOfFeedsAndAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [SetUp]
            public void When()
            {
                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = LiveStreams
                });
            }

            [Test]
            public void ItShouldAggregateOnTeams()
            {
                Assert.That(_hockeyStreams.Count(), Is.EqualTo(1));
                Assert.That(_hockeyStreams.ElementAt(0).HomeTeam, Is.EqualTo("New Jersey Devils"));
                Assert.That(_hockeyStreams.ElementAt(0).AwayTeam, Is.EqualTo("Tampa Bay Lightning"));
                Assert.That(_hockeyStreams.ElementAt(0).HomeStreamId, Is.EqualTo(34172));
                Assert.That(_hockeyStreams.ElementAt(0).AwayStreamId, Is.EqualTo(34173));
            }
        }
    }
}