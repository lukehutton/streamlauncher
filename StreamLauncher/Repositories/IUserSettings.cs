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
        bool IsFirstRun { get; set; }
        string PreferredLocation { get; set; }
        string PreferredEventType { get; set; }
        int? RtmpTimeOutInSeconds { get; set; }
        bool? ShowScoring { get; set; }
        string PreferredQuality { get; set; }
        void Save();
    }
}