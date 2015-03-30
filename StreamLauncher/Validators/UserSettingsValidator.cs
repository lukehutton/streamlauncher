using System.Collections.Generic;
using StreamLauncher.Repositories;
using StreamLauncher.Util;

namespace StreamLauncher.Validators
{
    public class UserSettingsValidator : IUserSettingsValidator
    {
        private readonly IFileHelper _fileHelper;

        public UserSettingsValidator(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public IEnumerable<string> BrokenRules(IUserSettings userSettings)
        {
            if (userSettings.LiveStreamerPath.IsNullOrEmpty())
            {
                yield return "Livestreamer Path must not be empty.";                
            }

            if (userSettings.MediaPlayerPath.IsNullOrEmpty())
            {
                yield return "Media Player Path must not be empty.";                
            }

            if (!_fileHelper.FileExistsCaseSensitive(userSettings.LiveStreamerPath))
            {
                yield return "Livestreamer Path does not exist. Livestreamer executable name is case sensitive.";

            }

            if (!_fileHelper.FileExists(userSettings.MediaPlayerPath))
            {
                yield return "Media Player Path does not exist.";
            }

            if (userSettings.RtmpTimeOutInSeconds < 1)
            {
                yield return "RTMP Timeout (seconds) must be greater than 0.";
            }
        }         
    }
}