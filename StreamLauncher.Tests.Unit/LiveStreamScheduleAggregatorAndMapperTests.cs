using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Dtos;
using StreamLauncher.Mappers;
using StreamLauncher.Models;

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
                        StartTime = "4:00 PM PST",
                        AwayTeam = "Tampa Bay Lightning",
                        HomeTeam = "New Jersey Devils",
                        FeedType = "Away Feed",
                        AwayScore = "5",
                        HomeScore = "2",
                        IsPlaying = "1",
                        Event = "NHL"
                    },
                    new LiveStreamDto
                    {
                        Id = "34175",
                        StartTime = "5:00 PM PST",
                        AwayTeam = "Vancouver Canucks",
                        HomeTeam = "Toronto Maple Leafs",
                        FeedType = "Away Feed",
                        AwayScore = "3",
                        HomeScore = "2",
                        IsPlaying = "1",
                        Event = "NHL"
                    },
                    new LiveStreamDto
                    {
                        Id = "34176",
                        StartTime = "5:00 PM PST",
                        AwayTeam = "Vancouver Canucks",
                        HomeTeam = "Toronto Maple Leafs",
                        FeedType = "Home Feed",
                        AwayScore = "3",
                        HomeScore = "2",
                        IsPlaying = "1",
                        Event = "NHL"
                    },
                    new LiveStreamDto
                    {
                        Id = "34172",
                        StartTime = "4:00 PM PST",
                        AwayTeam = "Tampa Bay Lightning",
                        HomeTeam = "New Jersey Devils",
                        FeedType = "Home Feed",
                        AwayScore = "5",
                        HomeScore = "2",
                        IsPlaying = "1",
                        Event = "NHL"
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
            public void ItShouldAggregateOnTeamsAndMapAndSort()
            {
                Assert.That(_hockeyStreams.Count(), Is.EqualTo(2));
                Assert.That(_hockeyStreams.ElementAt(0).HomeTeam, Is.EqualTo("New Jersey Devils"));
                Assert.That(_hockeyStreams.ElementAt(0).AwayTeam, Is.EqualTo("Tampa Bay Lightning"));
                Assert.That(_hockeyStreams.ElementAt(0).StartTime, Is.EqualTo("4:00 PM PST"));
                Assert.That(_hockeyStreams.ElementAt(0).HomeStreamId, Is.EqualTo(34172));
                Assert.That(_hockeyStreams.ElementAt(0).AwayStreamId, Is.EqualTo(34173));
                Assert.That(_hockeyStreams.ElementAt(0).Score, Is.EqualTo("2 - 5"));
                Assert.That(_hockeyStreams.ElementAt(0).HomeImagePath, Is.EqualTo(@"../Images/Teams/New Jersey Devils.png"));
                Assert.That(_hockeyStreams.ElementAt(0).AwayImagePath, Is.EqualTo(@"../Images/Teams/Tampa Bay Lightning.png"));
                Assert.That(_hockeyStreams.ElementAt(0).EventType, Is.EqualTo(EventType.NHL));
                Assert.That(_hockeyStreams.ElementAt(1).HomeTeam, Is.EqualTo("Toronto Maple Leafs"));
                Assert.That(_hockeyStreams.ElementAt(1).AwayTeam, Is.EqualTo("Vancouver Canucks"));
                Assert.That(_hockeyStreams.ElementAt(1).StartTime, Is.EqualTo("5:00 PM PST"));
                Assert.That(_hockeyStreams.ElementAt(1).HomeStreamId, Is.EqualTo(34176));
                Assert.That(_hockeyStreams.ElementAt(1).AwayStreamId, Is.EqualTo(34175));
                Assert.That(_hockeyStreams.ElementAt(1).Score, Is.EqualTo("2 - 3"));
                Assert.That(_hockeyStreams.ElementAt(1).HomeImagePath, Is.EqualTo(@"../Images/Teams/Toronto Maple Leafs.png"));
                Assert.That(_hockeyStreams.ElementAt(1).AwayImagePath, Is.EqualTo(@"../Images/Teams/Vancouver Canucks.png"));
                Assert.That(_hockeyStreams.ElementAt(1).EventType, Is.EqualTo(EventType.NHL));
            }
        }
    }
}