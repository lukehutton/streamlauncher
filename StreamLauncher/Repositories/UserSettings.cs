namespace StreamLauncher.Repositories
{
    public class UserSettings : IUserSettings
    {
        public string UserName
        {
            get { return Properties.Settings.Default.UserName; }
            set { Properties.Settings.Default.UserName = value; }
        }

        public string EncryptedPassword
        {
            get { return Properties.Settings.Default.EncryptedPassword; }
            set { Properties.Settings.Default.EncryptedPassword = value; }
        }

        public bool RememberMe
        {
            get { return Properties.Settings.Default.RememberMe; }
            set { Properties.Settings.Default.RememberMe = value; }
        }

        public string LiveStreamerPath
        {
            get { return Properties.Settings.Default.LiveStreamerPath; }
            set { Properties.Settings.Default.LiveStreamerPath = value; }
        }

        public string MediaPlayerPath
        {
            get { return Properties.Settings.Default.MediaPlayerPath; }
            set { Properties.Settings.Default.MediaPlayerPath = value; }
        }
        public string MediaPlayerArguments
        {
            get { return Properties.Settings.Default.MediaPlayerArguments; }
            set { Properties.Settings.Default.MediaPlayerArguments = value; }
        }

        public bool IsFirstRun
        {
            get { return Properties.Settings.Default.IsFirstRun; }
            set { Properties.Settings.Default.IsFirstRun = value; }
        }

        public string PreferredLocation
        {
            get { return Properties.Settings.Default.PreferredLocation; }
            set { Properties.Settings.Default.PreferredLocation = value; }
        }

        public string PreferredEventType
        {
            get { return Properties.Settings.Default.PreferredEventType; }
            set { Properties.Settings.Default.PreferredEventType = value; }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}