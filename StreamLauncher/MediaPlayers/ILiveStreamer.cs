using StreamLauncher.Repositories;

namespace StreamLauncher.MediaPlayers
{
    public interface ILiveStreamer
    {
        void Play(string streamSource, Quality quality);
        void SaveConfig(string mediaPlayerPath, string mediaPlayerArguments);
    }
}