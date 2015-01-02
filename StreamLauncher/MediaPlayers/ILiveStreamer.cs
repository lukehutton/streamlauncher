using System.Security.Cryptography.X509Certificates;

namespace StreamLauncher.MediaPlayers
{
    public interface ILiveStreamer
    {
        void Play(string source);
    }
}