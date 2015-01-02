using System.Diagnostics;

namespace StreamLauncher.MediaPlayers
{
    public class LiveStreamer : ILiveStreamer
    {
        public void Play(string source)
        {
            Process process = new Process
            {
                StartInfo =
                {                    
                    FileName = "cmd.exe",
                    Arguments = string.Format("/c livestreamer \"{0}\" \"best\"", source.Replace("'", string.Empty)),
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
        }
    }
}