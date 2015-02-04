using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Dtos;
using StreamLauncher.Mappers;
using StreamLauncher.Models;

namespace StreamLauncher.Tests.Unit.Mappers
{
    [TestFixture]
    public class LiveStreamScheduleAggregatorAndMapperTests
    {
        public class GivenAnAggregator
        {
            protected ILiveStreamScheduleAggregatorAndMapper LiveStreamScheduleAggregatorAndMapper;

            [TestFixtureSetUp]
            public void Given()
            {
                LiveStreamScheduleAggregatorAndMapper = new LiveStreamScheduleAggregatorAndMapper();
            }
        }

        public class WhenAggregateTwoHomeAndAwayFeeds : GivenAnAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [TestFixtureSetUp]
            public void When()
            {
                var liveStreams = BuildLiveStreams();

                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = liveStreams
                });
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
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

            [Test]
            public void ItShouldMapAllFields()
            {
                var stream = _hockeyStreams.First();
                Assert.That(stream.Feeds.Count(x => x.StreamId == 34172), Is.EqualTo(1));
                Assert.That(stream.Feeds.Count(x => x.StreamId == 34173), Is.EqualTo(1));
                Assert.That(stream.Feeds.Count(x => x.FeedType == "New Jersey Devils Feed"), Is.EqualTo(1));
                Assert.That(stream.Feeds.Count(x => x.FeedType == "Tampa Bay Lightning Feed"), Is.EqualTo(1));                                
                Assert.That(stream.Feeds.Count(x => x.IsPlaying), Is.EqualTo(2));
                Assert.That(stream.Feeds.Count(x => x.Game == "Tampa Bay Lightning at New Jersey Devils"), Is.EqualTo(2));                                
                Assert.That(stream.HomeTeam, Is.EqualTo("New Jersey Devils"));
                Assert.That(stream.AwayTeam, Is.EqualTo("Tampa Bay Lightning"));
                Assert.That(stream.StartTime, Is.EqualTo("4:00 PM PST"));
                Assert.That(stream.Score, Is.EqualTo("2 - 5"));
                Assert.That(stream.IsPlaying, Is.True);
                Assert.That(stream.HomeImagePath, Is.EqualTo(@"../Images/Teams/New Jersey Devils.png"));
                Assert.That(stream.AwayImagePath, Is.EqualTo(@"../Images/Teams/Tampa Bay Lightning.png"));
                Assert.That(stream.EventType, Is.EqualTo(EventType.NHL));
            }
        }

        public class WhenAggregateSingleHomeFeed : GivenAnAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [TestFixtureSetUp]
            public void When()
            {
                var liveStreams = BuildLiveStreams();

                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = liveStreams
                });
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
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
                    }
                };
            }

            [Test]
            public void ItShouldReturnFeedThatHasOnlyHomeFeed()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34181), Is.EqualTo(1));
            }
        }

        public class WhenAggregateSingleAwayFeed : GivenAnAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [TestFixtureSetUp]
            public void When()
            {
                var liveStreams = BuildLiveStreams();

                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = liveStreams
                });
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
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
                    }
                };
            }

            [Test]
            public void ItShouldReturnFeedThatHasOnlyAwayFeed()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34179), Is.EqualTo(1));
            }
        }

        public class WhenAggregateNullFeed : GivenAnAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [TestFixtureSetUp]
            public void When()
            {
                var liveStreams = BuildLiveStreams();

                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = liveStreams
                });
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
                    new LiveStreamDto // null feed type
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
                    }
                };
            }

            [Test]
            public void ItShouldReturnFeedThatHasNullFeedType()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34180), Is.EqualTo(1));                
            }
            
            [Test]
            public void ItShouldReturnCorrectPlayFeedText()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.FeedType == "Play Feed"), Is.EqualTo(1));
            }
        }

        public class WhenAggregateUnknownFeedTypeAndAwayFeedType : GivenAnAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [TestFixtureSetUp]
            public void When()
            {
                var liveStreams = BuildLiveStreams();

                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = liveStreams
                });
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
                    new LiveStreamDto // unknown feed type
                    {
                        Id = "34189",
                        StartTime = "7:45 PM PST",
                        AwayTeam = "Calgary Flames",
                        HomeTeam = "New York Islanders",
                        FeedType = "NBC Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    },
                    new LiveStreamDto // away feed type
                    {
                        Id = "34190",
                        StartTime = "7:45 PM PST",
                        AwayTeam = "Calgary Flames",
                        HomeTeam = "New York Islanders",
                        FeedType = "Away Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    }
                };
            }

            [Test]
            public void ItShouldReturnFeedThatHasUnknownAndAwayFeedType()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34189), Is.EqualTo(1));
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34190), Is.EqualTo(1));                
            }

            [Test]
            public void ItShouldReturnCorrectPlayFeedText()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.FeedType == "NBC Feed"), Is.EqualTo(1));
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.FeedType == "Calgary Flames Feed"), Is.EqualTo(1));
            }
        }
        public class WhenAggregateUnknownFeedTypeAndHomeFeedType : GivenAnAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [TestFixtureSetUp]
            public void When()
            {
                var liveStreams = BuildLiveStreams();

                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = liveStreams
                });
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
                    new LiveStreamDto // unknown feed type
                    {
                        Id = "34189",
                        StartTime = "7:45 PM PST",
                        AwayTeam = "Calgary Flames",
                        HomeTeam = "New York Islanders",
                        FeedType = "NBC Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    },
                    new LiveStreamDto // home feed type
                    {
                        Id = "34190",
                        StartTime = "7:45 PM PST",
                        AwayTeam = "Calgary Flames",
                        HomeTeam = "New York Islanders",
                        FeedType = "Home Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    }
                };
            }

            [Test]
            public void ItShouldReturnFeedThatHasUnknownAndAwayFeedType()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34189), Is.EqualTo(1));
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34190), Is.EqualTo(1));      
            }

            [Test]
            public void ItShouldReturnCorrectPlayFeedText()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.FeedType == "NBC Feed"), Is.EqualTo(1));
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.FeedType == "New York Islanders Feed"), Is.EqualTo(1));
            }
        }

        public class WhenAggregateTwoUnknownFeedType : GivenAnAggregator
        {
            private IEnumerable<HockeyStream> _hockeyStreams;

            [TestFixtureSetUp]
            public void When()
            {
                var liveStreams = BuildLiveStreams();

                _hockeyStreams = LiveStreamScheduleAggregatorAndMapper.AggregateAndMap(new GetLiveStreamsResponseDto
                {
                    Schedule = liveStreams
                });
            }

            private List<LiveStreamDto> BuildLiveStreams()
            {
                return new List<LiveStreamDto>
                {
                    new LiveStreamDto // unknown feed type
                    {
                        Id = "34189",
                        StartTime = "7:45 PM PST",
                        AwayTeam = "Calgary Flames",
                        HomeTeam = "New York Islanders",
                        FeedType = "NBC Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    },
                    new LiveStreamDto // unknown feed type
                    {
                        Id = "34190",
                        StartTime = "7:45 PM PST",
                        AwayTeam = "Calgary Flames",
                        HomeTeam = "New York Islanders",
                        FeedType = "RDS Feed",
                        AwayScore = "0",
                        HomeScore = "0",
                        IsPlaying = "0",
                        Event = "NHL"
                    }
                };
            }

            [Test]
            public void ItShouldReturnFeedThatHasUnknownAndAwayFeedType()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34189), Is.EqualTo(1));
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.StreamId == 34190), Is.EqualTo(1));      
            }

            [Test]
            public void ItShouldReturnCorrectPlayFeedText()
            {
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.FeedType == "NBC Feed"), Is.EqualTo(1));
                Assert.That(_hockeyStreams.First().Feeds.Count(x => x.FeedType == "RDS Feed"), Is.EqualTo(1));                
            }
        }
    }
}