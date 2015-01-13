
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Api;
using StreamLauncher.Models;
using StreamLauncher.Repositories;

namespace StreamLauncher.Tests.Integration
{
    [TestFixture]
    public class ScoresRepositoryTests
    {
        public class GivenAScoresRepository
        {
            protected IScoresRepository ScoresRepository;

            [TestFixtureSetUp]
            public void Given()
            {
                var apiKeyProvider = new ApiKeyProvider();                
                var hockeyStreamsApi = new HockeyStreamsApiRequiringScoresApiKey(apiKeyProvider);
                ScoresRepository = new ScoresRepository(hockeyStreamsApi);
            }
        }

        public class WhenGetScores: GivenAScoresRepository
        {
            private IEnumerable<Score> _scores;

            [SetUp]
            public void When()
            {
                _scores = ScoresRepository.GetScores();
            }

            [Test]
            public void ItShouldReturnAllTheScores()
            {
                Assert.That(_scores.Count(), Is.GreaterThan(0));
            }
        }
    }
}
