using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Dtos;
using StreamLauncher.Mappers;
using StreamLauncher.Models;

namespace StreamLauncher.Tests.Unit
{
    [TestFixture]
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
            public void ItShouldMapAllFields()
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
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveHomeAndAwayFeeds()
            {                
                Assert.That(_hockeyStreams.ElementAt(0).HomeStreamId, Is.EqualTo(34172));
                Assert.That(_hockeyStreams.ElementAt(0).AwayStreamId, Is.EqualTo(34173));
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveNoFeedTypes()
            {                
                Assert.That(_hockeyStreams.ElementAt(3).HomeStreamId, Is.EqualTo(34180));
                Assert.That(_hockeyStreams.ElementAt(3).AwayStreamId, Is.EqualTo(0));
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveOnlyAwayFeeds()
            {             
                Assert.That(_hockeyStreams.ElementAt(2).HomeStreamId, Is.EqualTo(0));
                Assert.That(_hockeyStreams.ElementAt(2).AwayStreamId, Is.EqualTo(34179));
            }

            [Test]
            public void ItShouldReturnFeedsThatHaveOnlyHomeFeeds()
            {                
                Assert.That(_hockeyStreams.ElementAt(4).HomeStreamId, Is.EqualTo(34181));
                Assert.That(_hockeyStreams.ElementAt(4).AwayStreamId, Is.EqualTo(0));
            }
        }
    }
}