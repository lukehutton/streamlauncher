using System.Diagnostics;

namespace StreamLauncher.MediaPlayers
{
    public class LiveStreamer : ILiveStreamer
    {
        public void Play(string source)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = string.Format(@"/c c:\progra~2\livestreamer\livestreamer.exe " + "\"{0}\" \"best\"", source),                    
                    UseShellExecute = false
                }
            };
            process.Start();
        }
    }
}