using System.Diagnostics;
using System.IO;
using StreamLauncher.Exceptions;
using StreamLauncher.Repositories;

namespace StreamLauncher.MediaPlayers
{
    public class LiveStreamer : ILiveStreamer
    {
        private readonly IUserSettings _userSettings;

        public static string Default64BitLocation = @"C:\Program Files (x86)\Livestreamer\livestreamer.exe";
        public static string Default32BitLocation = @"C:\Program Files\Livestreamer\livestreamer.exe";

        public LiveStreamer(IUserSettings userSettings)
        {
            _userSettings = userSettings;
        }

        public void Play(string streamSource, Quality quality)
        {
            if (!File.Exists(_userSettings.LiveStreamerPath))
            {
                throw new LiveStreamerExecutableNotFound();
            }
            if (!File.Exists(_userSettings.MediaPlayerPath))
            {
                throw new MediaPlayerNotFound();
            }

            var arguments = string.Format("/c \"\"{0}\" \"{1}\" \"{2}\"\"", _userSettings.LiveStreamerPath, streamSource,
                quality == Quality.HD ? "best" : "worst");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = arguments,                    
                    UseShellExecute = false
                }
            };
            process.Start();
        }
    }
}