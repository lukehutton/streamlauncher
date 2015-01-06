using System.Collections.Generic;
using StreamLauncher.Repositories;

namespace StreamLauncher.Validators
{
    public interface IUserSettingsValidator
    {
        IEnumerable<string> BrokenRules(IUserSettings userSettings);
    }
}