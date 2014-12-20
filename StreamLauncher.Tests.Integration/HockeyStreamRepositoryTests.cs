﻿
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using StreamLauncher.Models;
using StreamLauncher.Providers;
using StreamLauncher.Repositories;
using StreamLauncher.Services;

namespace StreamLauncher.Tests.Integration
{
    public class HockeyStreamRepositoryTests
    {
        public class GivenAnAuthenticatedUser
        {
            private IAuthenticationService _authenticationService;
            
            private string _userName;
            private string _password;

            protected User AuthenticatedUser;

            [TestFixtureSetUp]
            public void Given()
            {
                _userName = Convert.ToString(ConfigurationManager.AppSettings["hockeystreams.userName"]);
                _password = Convert.ToString(ConfigurationManager.AppSettings["hockeystreams.password"]);

                var apiKeyProvider = new ApiKeyProvider();
                var hockeyStreamsApi = new HockeyStreamsApiRequiringApiKey(apiKeyProvider);
                _authenticationService = new AuthenticationService(hockeyStreamsApi);
                var result = _authenticationService.Authenticate(_userName, _password);
                if (result.IsAuthenticated)
                {
                    AuthenticatedUser = result.AuthenticatedUser;
                }
                else
                {
                    Assert.Fail("User could not be authenticated");
                }
            }   
        }

        public class GivenAnAuthenticationToken : GivenAnAuthenticatedUser
        {
            protected ITokenProvider TokenProvider;

            [TestFixtureSetUp]
            public new void Given()
            {
                TokenProvider = new AuthenticationTokenProvider {Token = AuthenticatedUser.Token};
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

            [Test, Ignore("TODO")]
            public void ItShouldReturnAtLeastOneStream()
            {
                Assert.That(_streams.Count(), Is.GreaterThan(0));  
            }
        }
    }
}