namespace StreamLauncher.Repositories
{
    public interface IUserSettings
    {
        string UserName { get; set; }
        string EncryptedPassword { get; set; }
        bool RememberMe { get; set; }
        string LiveStreamerPath { get; set; }
        string MediaPlayerPath { get; set; }
        string MediaPlayerArguments { get; set; }
        void Save();
    }
}