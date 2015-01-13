using StreamLauncher.Properties;
using StreamLauncher.Util;

namespace StreamLauncher.Repositories
{
    public class UserSettings : IUserSettings
    {
        public string UserName
        {
            get { return Settings.Default.UserName; }
            set { Settings.Default.UserName = value; }
        }

        public string EncryptedPassword
        {
            get { return Settings.Default.EncryptedPassword; }
            set { Settings.Default.EncryptedPassword = value; }
        }

        public bool RememberMe
        {
            get { return Settings.Default.RememberMe; }
            set { Settings.Default.RememberMe = value; }
        }

        public string LiveStreamerPath
        {
            get { return Settings.Default.LiveStreamerPath; }
            set { Settings.Default.LiveStreamerPath = value; }
        }

        public string MediaPlayerPath
        {
            get { return Settings.Default.MediaPlayerPath; }
            set { Settings.Default.MediaPlayerPath = value; }
        }

        public string MediaPlayerArguments
        {
            get { return Settings.Default.MediaPlayerArguments; }
            set { Settings.Default.MediaPlayerArguments = value; }
        }

        public bool IsFirstRun
        {
            get { return Settings.Default.IsFirstRun; }
            set { Settings.Default.IsFirstRun = value; }
        }

        public string PreferredLocation
        {
            get
            {
                return Settings.Default.PreferredLocation.IsNullOrEmpty()
                    ? "North America - West"
                    : Settings.Default.PreferredLocation;
            }
            set { Settings.Default.PreferredLocation = value; }
        }

        public string PreferredEventType
        {
            get
            {
                return Settings.Default.PreferredEventType.IsNullOrEmpty() ? "NHL" : Settings.Default.PreferredEventType;
            }
            set { Settings.Default.PreferredEventType = value; }
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}