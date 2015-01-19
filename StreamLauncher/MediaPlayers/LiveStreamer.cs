using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using StreamLauncher.Exceptions;
using StreamLauncher.Messages;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Util;

namespace StreamLauncher.MediaPlayers
{
    public class LiveStreamer : ILiveStreamer
    {        
        public static string Default64BitLocation = @"C:\Program Files (x86)\Livestreamer\livestreamer.exe";
        public static string Default32BitLocation = @"C:\Program Files\Livestreamer\livestreamer.exe";
        public static string RtmpDumpRelativePath = @"rtmpdump\rtmpdump.exe";

        private readonly StringBuilder _output = new StringBuilder();

        private readonly IUserSettings _userSettings;
        private readonly IDialogService _dialogService;
        private readonly IMessengerService _messengerService;
        private readonly IFileHelper _fileHelper;

        public LiveStreamer(IUserSettings userSettings, IDialogService dialogService, IMessengerService messengerService, IFileHelper fileHelper)
        {
            _userSettings = userSettings;
            _dialogService = dialogService;
            _messengerService = messengerService;
            _fileHelper = fileHelper;
        }

        public void Play(string game, string streamSource, Quality quality)
        {
            if (!_fileHelper.FileExists(_userSettings.LiveStreamerPath))
            {
                throw new LiveStreamerExecutableNotFound();
            }
            if (!_fileHelper.FileExists(_userSettings.MediaPlayerPath))
            {
                throw new MediaPlayerNotFound();
            }

            _messengerService.Send(new BusyStatusMessage(true, "Playing stream..."));

            Task.Run(() =>
            {
                _output.Clear();

                var qualityString = quality == Quality.HD ? "best" : "worst";
                var arguments = string.Format(
                    "/c \"\"{0}\" \"{1}\" \"{2}\"\" --rtmp-timeout {3}",
                    _userSettings.LiveStreamerPath,
                    streamSource,
                    qualityString,
                    _userSettings.RtmpTimeOutInSeconds);

                using (var process = new ProcessUtil("cmd.exe", arguments))
                {
                    process.Start();
                    process.OutputDataReceived += OutputDataReceived;
                    process.ErrorDataReceived += ErrorDataReceived;
                    process.Wait();
                    if (process.ExitCode != 0 || _output.ToString().Contains("error"))
                    {
                        throw new LiveStreamerError(game);
                    }
                }
            }).ContinueWith(o => MyErrorHandler(o.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SaveConfig()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var livestreamerConfig = Path.Combine(appDataPath, "livestreamer", "livestreamerrc");

            var rtmpPath = Path.Combine(Path.GetDirectoryName(_userSettings.LiveStreamerPath), RtmpDumpRelativePath);

            var configBuilder = new StringBuilder();
            configBuilder.AppendFormat("player=\"{0}\" {1}", _userSettings.MediaPlayerPath,
                _userSettings.MediaPlayerArguments);
            configBuilder.AppendLine();
            configBuilder.AppendFormat("rtmpdump={0}", rtmpPath);
            configBuilder.AppendLine();
            configBuilder.AppendLine("player-no-close");

            File.WriteAllText(livestreamerConfig, configBuilder.ToString());
        }

        private void MyErrorHandler(AggregateException exception)
        {         
            if (exception.InnerException is LiveStreamerError)
            {
                _dialogService.ShowMessage(
                    string.Format("No live feed for {0} available.", exception.InnerException.Message), "Error", "OK");                
            }
            _messengerService.Send(new BusyStatusMessage(false, ""));
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("Starting player"))
                {
                    _messengerService.Send(new BusyStatusMessage(false, ""));
                }
                _output.AppendLine(e.Data);
            }
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _output.AppendLine(e.Data);
            }
        }
    }
}