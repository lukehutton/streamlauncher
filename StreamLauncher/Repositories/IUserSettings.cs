namespace StreamLauncher.Repositories
{
    public interface IUserSettings
    {
        string UserName { get; set; }
        string EncryptedPassword { get; set; }
        bool RememberMe { get; set; }
        void Save();
    }
}