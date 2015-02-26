using System;
using NUnit.Framework;
using StreamLauncher.Mappers;
using StreamLauncher.Models;

namespace StreamLauncher.Tests.Unit.Mappers
{
    [TestFixture]
    public class ExtractTimeOfDayFromStreamTests
    {
        public class GivenAnExtractTimeOfDayFromStream
        {
            protected IExtractTimeOfDayFromStream ExtractTimeOfDayFromStream;

            [TestFixtureSetUp]
            public void Given()
            {
                ExtractTimeOfDayFromStream = new ExtractTimeOfDayFromStream();
            }
        }

        public class WhenExtractTimeOfDayFrom24HourTime : GivenAnExtractTimeOfDayFromStream
        {
            private TimeSpan _timeOfDay;

            [TestFixtureSetUp]
            public void When()
            {
                var stream = new HockeyStream
                {
                    StartTime = "15:00 HAST"
                };
                _timeOfDay = ExtractTimeOfDayFromStream.ExtractTimeOfDay(stream);
            }

            [Test]
            public void ItShouldSetTimeOfDay()
            {
                Assert.That(_timeOfDay, Is.EqualTo(TimeSpan.FromHours(15)));
            }
        }   
        
        public class WhenExtractTimeOfDayFromNon24HourTime : GivenAnExtractTimeOfDayFromStream
        {
            private TimeSpan _timeOfDay;

            [TestFixtureSetUp]
            public void When()
            {
                var stream = new HockeyStream
                {
                    StartTime = "10:00 AM PST"
                };
                _timeOfDay = ExtractTimeOfDayFromStream.ExtractTimeOfDay(stream);
            }

            [Test]
            public void ItShouldSetTimeOfDay()
            {
                Assert.That(_timeOfDay, Is.EqualTo(TimeSpan.FromHours(10)));
            }
        }
        
        public class WhenExtractTimeOfDayFromBadTime : GivenAnExtractTimeOfDayFromStream
        {
            private TimeSpan _timeOfDay;

            [TestFixtureSetUp]
            public void When()
            {
                var stream = new HockeyStream
                {
                    StartTime = "bad time"
                };
                _timeOfDay = ExtractTimeOfDayFromStream.ExtractTimeOfDay(stream);
            }

            [Test]
            public void ItShouldDefaultTimeOfDayToMidnight()
            {
                Assert.That(_timeOfDay, Is.EqualTo(TimeSpan.FromHours(0)));
            }
        }
    }
}