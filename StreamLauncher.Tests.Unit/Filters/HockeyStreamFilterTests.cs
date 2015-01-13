using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Filters;
using StreamLauncher.Models;

namespace StreamLauncher.Tests.Unit.Filters
{
    [TestFixture]
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
                    new HockeyStream {EventType = EventType.AHL, IsPlaying = true, PeriodAndTimeLeft = "1st"},
                    new HockeyStream {EventType = EventType.NHL, IsPlaying = true, PeriodAndTimeLeft = "2nd"},
                    new HockeyStream {EventType = EventType.NHL, IsPlaying = false, PeriodAndTimeLeft = "Final"},
                    new HockeyStream {EventType = EventType.OHL, IsPlaying = false, PeriodAndTimeLeft = "-"},
                    new HockeyStream {EventType = EventType.WHL, IsPlaying = true, PeriodAndTimeLeft = "SO"}
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
        public class WhenFilterByComingSoon : GivenAHockeyStreamFilter
        {
            private IEnumerable<HockeyStream> _filteredHockeyStreams;

            [SetUp]
            public void When()
            {
                _filteredHockeyStreams = HockeyStreamFilter.By(HockeyStreams, new ActiveFilterSpecification("Coming Soon"));
            }

            [Test]
            public void ItShouldFilter()
            {
                Assert.That(_filteredHockeyStreams.Count(), Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class WhenFilterByCompleted : GivenAHockeyStreamFilter
        {
            private IEnumerable<HockeyStream> _filteredHockeyStreams;

            [SetUp]
            public void When()
            {
                _filteredHockeyStreams = HockeyStreamFilter.By(HockeyStreams, new ActiveFilterSpecification("Completed"));
            }

            [Test]
            public void ItShouldFilter()
            {
                Assert.That(_filteredHockeyStreams.Count(), Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class WhenFilterByInProgress : GivenAHockeyStreamFilter
        {
            private IEnumerable<HockeyStream> _filteredHockeyStreams;

            [SetUp]
            public void When()
            {
                _filteredHockeyStreams = HockeyStreamFilter.By(HockeyStreams, new ActiveFilterSpecification("In Progress"));
            }

            [Test]
            public void ItShouldFilter()
            {
                Assert.That(_filteredHockeyStreams.Count(), Is.EqualTo(3));
            }
        }
    }
}