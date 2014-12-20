
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Api;
using StreamLauncher.Models;
using StreamLauncher.Repositories;

namespace StreamLauncher.Tests.Integration
{
    public class StreamLocationRepositoryTests
    {
        public class GivenAStreamLocationRepository
        {
            protected IStreamLocationRepository StreamLocationRepository;

            [TestFixtureSetUp]
            public void Given()
            {
                var hockeyStreamsApi = new HockeyStreamsApi();
                StreamLocationRepository = new StreamLocationRepository(hockeyStreamsApi);
            }
        }

        public class WhenGetLocations : GivenAStreamLocationRepository
        {
            private IEnumerable<StreamLocation> _locations;

            [SetUp]
            public void When()
            {
                _locations = StreamLocationRepository.GetLocations();
            }

            [Test]
            public void ItShouldReturnAllTheLocationsOrdered()
            {
                Assert.That(_locations.Count(), Is.EqualTo(7));
                Assert.That(_locations.ElementAt(0).Location, Is.EqualTo("Asia"));
                Assert.That(_locations.ElementAt(1).Location, Is.EqualTo("Australia"));
                Assert.That(_locations.ElementAt(2).Location, Is.EqualTo("Europe"));
                Assert.That(_locations.ElementAt(3).Location, Is.EqualTo("North America - Central"));
                Assert.That(_locations.ElementAt(4).Location, Is.EqualTo("North America - East"));
                Assert.That(_locations.ElementAt(5).Location, Is.EqualTo("North America - East Canada"));
                Assert.That(_locations.ElementAt(6).Location, Is.EqualTo("North America - West"));
            }
        }
    }
}
