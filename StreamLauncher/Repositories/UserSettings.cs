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

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}