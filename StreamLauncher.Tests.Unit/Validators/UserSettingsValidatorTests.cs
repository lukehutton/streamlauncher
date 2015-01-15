using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StreamLauncher.Repositories;
using StreamLauncher.Util;
using StreamLauncher.Validators;

namespace StreamLauncher.Tests.Unit.Validators
{
    [TestFixture]
    public class UserSettingsValidatorTests
    {
        public class GivenAUserSettingsValidator
        {
            protected IFileHelper FileHelper;
            protected IUserSettingsValidator UserSettingsValidator;

            [TestFixtureSetUp]
            public void Given()
            {
                FileHelper = MockRepository.GenerateMock<IFileHelper>();
                UserSettingsValidator = new UserSettingsValidator(FileHelper);
            }
        }

        [TestFixture]
        public class WhenValidateWithInvalidRtmpTimeout : GivenAUserSettingsValidator
        {
            private IEnumerable<string> _brokenRules;

            [TestFixtureSetUp]
            public void When()
            {
                var settings = new UserSettings
                {
                    LiveStreamerPath = "live streamer",
                    MediaPlayerPath = "media player",
                    RtmpTimeOutInSeconds = -1
                };
                FileHelper.Expect(x => x.FileExists(settings.LiveStreamerPath)).Return(true);
                FileHelper.Expect(x => x.FileExists(settings.MediaPlayerPath)).Return(true);
                _brokenRules = UserSettingsValidator.BrokenRules(settings);
            }

            [Test]
            public void ItShouldReturnRuleBroken()
            {
                Assert.That(_brokenRules.First(), Is.EqualTo("RTMP Timeout (seconds) must be greater than 0."));
            }
        }
    }
}