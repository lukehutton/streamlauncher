using GalaSoft.MvvmLight;
using StreamLauncher.Repositories;

namespace StreamLauncher.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IUserSettings _userSettings;

        public SettingsViewModel(IUserSettings userSettings)
        {
            _userSettings = userSettings;
        }
    }
}