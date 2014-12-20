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
                    new LiveStreamDto // no feed type
                    {
                        Id = "34180",           
                        StartTime = "7:30 PM PST",
                        AwayTeam = "New York Rangers",
                        HomeTeam = "Minnesota Wild",
                        FeedType = null,
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    },
                    new LiveStreamDto // single home feed only
                    {
                        Id = "34181",           
                        StartTime = "8:00 PM PST",
                        AwayTeam = "San Jose Sharks",
                        HomeTeam = "Anaheim Ducks",
                        FeedType = "Home Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    },
                    new LiveStreamDto // single away feed only
                    {
                        Id = "34179",           
                        StartTime = "7:00 PM PST",
                        AwayTeam = "Boston Bruins",
                        HomeTeam = "Ottawa Senators",
                        FeedType = "Away Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    },
                    new LiveStreamDto // home and away feeds
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
                    new LiveStreamDto // home and away feeds
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
                    new LiveStreamDto // home and away feeds
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
                    new LiveStreamDto // home and away feeds
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
            public void ItShouldReturnFiveHockeyStreams()
            {
                Assert.That(_hockeyStreams.Count(), Is.EqualTo(5));
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveHomeAndAwayFeeds()
            {                
                Assert.That(_hockeyStreams.ElementAt(0).HomeTeam, Is.EqualTo("New Jersey Devils"));
                Assert.That(_hockeyStreams.ElementAt(0).AwayTeam, Is.EqualTo("Tampa Bay Lightning"));
                Assert.That(_hockeyStreams.ElementAt(0).StartTime, Is.EqualTo("4:00 PM PST"));
                Assert.That(_hockeyStreams.ElementAt(0).HomeStreamId, Is.EqualTo(34172));
                Assert.That(_hockeyStreams.ElementAt(0).AwayStreamId, Is.EqualTo(34173));
                Assert.That(_hockeyStreams.ElementAt(0).Score, Is.EqualTo("2 - 5"));
                Assert.That(_hockeyStreams.ElementAt(0).IsPlaying, Is.True);
                Assert.That(_hockeyStreams.ElementAt(0).HomeImagePath, Is.EqualTo(@"../Images/Teams/New Jersey Devils.png"));
                Assert.That(_hockeyStreams.ElementAt(0).AwayImagePath, Is.EqualTo(@"../Images/Teams/Tampa Bay Lightning.png"));
                Assert.That(_hockeyStreams.ElementAt(0).EventType, Is.EqualTo(EventType.NHL));
                Assert.That(_hockeyStreams.ElementAt(1).HomeTeam, Is.EqualTo("Toronto Maple Leafs"));
                Assert.That(_hockeyStreams.ElementAt(1).AwayTeam, Is.EqualTo("Vancouver Canucks"));
                Assert.That(_hockeyStreams.ElementAt(1).StartTime, Is.EqualTo("5:00 PM PST"));
                Assert.That(_hockeyStreams.ElementAt(1).HomeStreamId, Is.EqualTo(34176));
                Assert.That(_hockeyStreams.ElementAt(1).AwayStreamId, Is.EqualTo(34175));
                Assert.That(_hockeyStreams.ElementAt(1).Score, Is.EqualTo("2 - 3"));
                Assert.That(_hockeyStreams.ElementAt(1).IsPlaying, Is.True);
                Assert.That(_hockeyStreams.ElementAt(1).HomeImagePath, Is.EqualTo(@"../Images/Teams/Toronto Maple Leafs.png"));
                Assert.That(_hockeyStreams.ElementAt(1).AwayImagePath, Is.EqualTo(@"../Images/Teams/Vancouver Canucks.png"));
                Assert.That(_hockeyStreams.ElementAt(1).EventType, Is.EqualTo(EventType.NHL));
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveNoFeedTypes()
            {
                Assert.That(_hockeyStreams.ElementAt(3).HomeTeam, Is.EqualTo("Minnesota Wild"));
                Assert.That(_hockeyStreams.ElementAt(3).AwayTeam, Is.EqualTo("New York Rangers"));
                Assert.That(_hockeyStreams.ElementAt(3).StartTime, Is.EqualTo("7:30 PM PST"));
                Assert.That(_hockeyStreams.ElementAt(3).NoFeedTypeStreamId, Is.EqualTo(34180));
                Assert.That(_hockeyStreams.ElementAt(3).HomeStreamId, Is.EqualTo(0));
                Assert.That(_hockeyStreams.ElementAt(3).AwayStreamId, Is.EqualTo(0));
                Assert.That(_hockeyStreams.ElementAt(3).Score, Is.EqualTo("0 - 0"));
                Assert.That(_hockeyStreams.ElementAt(3).IsPlaying, Is.False);
                Assert.That(_hockeyStreams.ElementAt(3).HomeImagePath, Is.EqualTo(@"../Images/Teams/Minnesota Wild.png"));
                Assert.That(_hockeyStreams.ElementAt(3).AwayImagePath, Is.EqualTo(@"../Images/Teams/New York Rangers.png"));
                Assert.That(_hockeyStreams.ElementAt(3).EventType, Is.EqualTo(EventType.NHL));
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveOnlyAwayFeeds()
            {
                Assert.That(_hockeyStreams.ElementAt(2).HomeTeam, Is.EqualTo("Ottawa Senators"));
                Assert.That(_hockeyStreams.ElementAt(2).AwayTeam, Is.EqualTo("Boston Bruins"));
                Assert.That(_hockeyStreams.ElementAt(2).StartTime, Is.EqualTo("7:00 PM PST"));
                Assert.That(_hockeyStreams.ElementAt(2).NoFeedTypeStreamId, Is.EqualTo(0));
                Assert.That(_hockeyStreams.ElementAt(2).HomeStreamId, Is.EqualTo(0));
                Assert.That(_hockeyStreams.ElementAt(2).AwayStreamId, Is.EqualTo(34179));
                Assert.That(_hockeyStreams.ElementAt(2).Score, Is.EqualTo("0 - 0"));
                Assert.That(_hockeyStreams.ElementAt(2).IsPlaying, Is.False);
                Assert.That(_hockeyStreams.ElementAt(2).HomeImagePath, Is.EqualTo(@"../Images/Teams/Ottawa Senators.png"));
                Assert.That(_hockeyStreams.ElementAt(2).AwayImagePath, Is.EqualTo(@"../Images/Teams/Boston Bruins.png"));
                Assert.That(_hockeyStreams.ElementAt(2).EventType, Is.EqualTo(EventType.NHL));
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveOnlyHomeFeeds()
            {
                Assert.That(_hockeyStreams.ElementAt(4).HomeTeam, Is.EqualTo("Anaheim Ducks"));
                Assert.That(_hockeyStreams.ElementAt(4).AwayTeam, Is.EqualTo("San Jose Sharks"));
                Assert.That(_hockeyStreams.ElementAt(4).StartTime, Is.EqualTo("8:00 PM PST"));
                Assert.That(_hockeyStreams.ElementAt(4).NoFeedTypeStreamId, Is.EqualTo(0));
                Assert.That(_hockeyStreams.ElementAt(4).HomeStreamId, Is.EqualTo(34181));
                Assert.That(_hockeyStreams.ElementAt(4).AwayStreamId, Is.EqualTo(0));
                Assert.That(_hockeyStreams.ElementAt(4).Score, Is.EqualTo("0 - 0"));
                Assert.That(_hockeyStreams.ElementAt(4).IsPlaying, Is.False);
                Assert.That(_hockeyStreams.ElementAt(4).HomeImagePath, Is.EqualTo(@"../Images/Teams/Anaheim Ducks.png"));
                Assert.That(_hockeyStreams.ElementAt(4).AwayImagePath, Is.EqualTo(@"../Images/Teams/San Jose Sharks.png"));
                Assert.That(_hockeyStreams.ElementAt(4).EventType, Is.EqualTo(EventType.NHL));
            }
        }
    }
}