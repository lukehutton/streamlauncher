using System;
using System.Configuration;
using NUnit.Framework;
using StreamLauncher.Providers;
using StreamLauncher.Services;

namespace StreamLauncher.Tests.Integration
{
    public class AuthenticationServiceTests
    {
        public class GivenAnAuthenticationService
        {
            protected IAuthenticationService AuthenticationService;

            [TestFixtureSetUp]
            public void Given()
            {
                var apiKeyProvider = new ApiKeyProvider();
                var hockeyStreamsApi = new HockeyStreamsApiRequiringApiKey(apiKeyProvider);
                AuthenticationService = new AuthenticationService(hockeyStreamsApi);
            }
        }

        public class WhenAuthenticateValidUser : GivenAnAuthenticationService
        {
            private AuthenticationResult _result;

            [SetUp]
            public void When()
            {
                var userName = Convert.ToString(ConfigurationManager.AppSettings["hockeystreams.userName"]);
                var password = Convert.ToString(ConfigurationManager.AppSettings["hockeystreams.password"]);
                _result = AuthenticationService.Authenticate(userName, password);
            }

            [Test]
            public void ItShouldSucceedAuthentication()
            {
                Assert.That(_result.IsAuthenticated, Is.True);
            }

            [Test]
            public void ItShouldReturnAToken()
            {
                Assert.That(_result.AuthenticatedUser.Token.Length, Is.GreaterThan(10));
            }

            [Test]
            public void ItShouldReturnFavoriteTeam()
            {
                Assert.That(_result.AuthenticatedUser.FavoriteTeam, Is.EqualTo("Vancouver Canucks"));
            }
        }
        public class WhenAuthenticateInvalidUser : GivenAnAuthenticationService
        {
            private AuthenticationResult _result;

            [SetUp]
            public void When()
            {
                const string userName = "badUserTest";
                const string password = "badPasswordTest";
                _result = AuthenticationService.Authenticate(userName, password);
            }

            [Test]
            public void ItShouldFailAuthentication()
            {
                Assert.That(_result.IsAuthenticated, Is.False);
            }

            [Test]
            public void ItShouldReturnBadUserNameOrPasswordMessage()
            {
                Assert.That(_result.ErrorMessage, Is.EqualTo("Invalid Username or Password"));
            }
        }
    }

}