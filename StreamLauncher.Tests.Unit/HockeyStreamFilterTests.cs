using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Filters;
using StreamLauncher.Models;

namespace StreamLauncher.Tests.Unit
{
    public class HockeyStreamFilterTests
    {
        public class GivenAHockeyStreamFilter
        {
            protected HockeyStreamFilter HockeyStreamFilter;
            protected IList<HockeyStream> HockeyStreams;

            [TestFixtureSetUp]
            public void Given()
            {
                HockeyStreamFilter = new HockeyStreamFilter();
                HockeyStreams = BuildHockeyStreams();
            }

            private IList<HockeyStream> BuildHockeyStreams()
            {
                return new List<HockeyStream>
                {
                    new HockeyStream {EventType = EventType.AHL, IsPlaying = true},
                    new HockeyStream {EventType = EventType.NHL, IsPlaying = true},
                    new HockeyStream {EventType = EventType.NHL, IsPlaying = false},
                    new HockeyStream {EventType = EventType.OHL, IsPlaying = false},
                    new HockeyStream {EventType = EventType.WHL, IsPlaying = true}
                };
            }
        }

        [TestFixture]
        public class WhenFilterByEventType : GivenAHockeyStreamFilter
        {
            private IEnumerable<HockeyStream> _filteredHockeyStreams;

            [SetUp]
            public void When()
            {
                _filteredHockeyStreams = HockeyStreamFilter.By(HockeyStreams, new EventTypeFilterSpecification(EventType.NHL));
            }

            [Test]
            public void ItShouldFilterByGivenEventType()
            {
                Assert.That(_filteredHockeyStreams.Count(), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class WhenFilterByActive : GivenAHockeyStreamFilter
        {
            private IEnumerable<HockeyStream> _filteredHockeyStreams;

            [SetUp]
            public void When()
            {
                _filteredHockeyStreams = HockeyStreamFilter.By(HockeyStreams, new ActiveFilterSpecification(true));
            }

            [Test]
            public void ItShouldFilterByGivenActiveState()
            {
                Assert.That(_filteredHockeyStreams.Count(), Is.EqualTo(3));
            }
        }
    }
}