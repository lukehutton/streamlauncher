using System.Collections.Generic;
using System.IO;
using StreamLauncher.Repositories;
using StreamLauncher.Util;

namespace StreamLauncher.Validators
{
    public class UserSettingsValidator : IUserSettingsValidator
    {
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

            if (!File.Exists(userSettings.LiveStreamerPath))
            {
                yield return "Livestreamer Path does not exist.";

            }

            if (!File.Exists(userSettings.MediaPlayerPath))
            {
                yield return "Media Player Path does not exist.";
            }
        }         
    }
}