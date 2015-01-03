using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using StreamLauncher.Exceptions;
using StreamLauncher.Repositories;
using StreamLauncher.Util;

namespace StreamLauncher.MediaPlayers
{
    public class LiveStreamer : ILiveStreamer
    {
        public static string Default64BitLocation = @"C:\Program Files (x86)\Livestreamer\livestreamer.exe";
        public static string Default32BitLocation = @"C:\Program Files\Livestreamer\livestreamer.exe";
        private readonly StringBuilder _output = new StringBuilder();
        private readonly IUserSettings _userSettings;

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
            var process = new ProcessUtil("cmd.exe", arguments);
            process.Start();
            _output.Clear();
            process.OutputDataReceived += OutputDataReceived;
            process.ErrorDataReceived += ErrorDataReceived;
            process.Wait();
            var exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                throw new LiveStreamerError();
            }
            if (_output.ToString().Contains("error"))
            {
                throw new LiveStreamerError();
            }
        }

        public void SaveConfig(string mediaPlayerPath, string mediaPlayerArguments)
        {
            var livestreamerConfig = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "livestreamer", "livestreamerrc");
            File.WriteAllText(livestreamerConfig,
                string.Format("player=\"{0}\" {1}", mediaPlayerPath, mediaPlayerArguments));
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _output.Append(e.Data);
            }
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _output.Append(e.Data);
            }
        }
    }
}