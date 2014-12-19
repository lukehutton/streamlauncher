
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Models;
using StreamLauncher.Providers;
using StreamLauncher.Repositories;

namespace StreamLauncher.Tests.Integration
{
    public class HockeyStreamRepositoryTests
    {
        public class GivenAnAuthenticationToken
        {
            protected ITokenProvider TokenProvider;

            [TestFixtureSetUp]
            public void Given()
            {
                TokenProvider = new AuthenticationTokenProvider();  
                // todo login
                // todo set token
                string token = "todo";
                TokenProvider.SetAuthenticationToken(token);
            }
        }

        public class GivenAHockeyStreamRepository : GivenAnAuthenticationToken
        {
            protected IHockeyStreamRepository HockeyStreamRepository;

            [TestFixtureSetUp]
            public new void Given()
            {                
                var hockeyStreamsApi = new HockeyStreamsApiRequiringToken(TokenProvider);
                HockeyStreamRepository = new HockeyStreamRepository(hockeyStreamsApi);
            }
        }

        public class WhenGetLiveStreams : GivenAHockeyStreamRepository
        {
            private IEnumerable<HockeyStream> _streams;

            [SetUp]
            public void When()
            {
                var task = HockeyStreamRepository.GetLiveStreams();
                _streams = task.Result;
            }

            [Test]
            public void ItShouldReturnAtLeastOneStream()
            {
                Assert.That(_streams.Count(), Is.GreaterThan(0));  
            }
        }
    }
}
