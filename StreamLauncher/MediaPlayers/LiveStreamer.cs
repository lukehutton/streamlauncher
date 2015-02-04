using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string _game;

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
                var message = string.Format("Live Streamer at path {0} not found.", _userSettings.LiveStreamerPath);
                var exception = new LiveStreamerExecutableNotFound(message);        
                Log.Error(exception.Message, exception);
                throw exception;
            }
            if (!_fileHelper.FileExists(_userSettings.MediaPlayerPath))
            {
                var message = string.Format("Media Player at path {0} not found.", _userSettings.MediaPlayerPath);
                var exception = new MediaPlayerNotFound(message);
                Log.Error(exception.Message, exception);
                throw exception;
            }

            _messengerService.Send(new BusyStatusMessage(true, "Playing stream..."), MessengerTokens.ChooseFeedsViewModelToken);

            _game = game;

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
                Log.InfoFormat("Attempting to play stream {0} with cmd.exe {1} using media player at {2}.", game, arguments, _userSettings.MediaPlayerPath);
                using (var process = new ProcessUtil("cmd.exe", arguments))
                {
                    process.Start();
                    process.OutputDataReceived += OutputDataReceived;
                    process.ErrorDataReceived += ErrorDataReceived;
                    process.Wait();
                    if (process.ExitCode != 0 || _output.ToString().Contains("error"))
                    {
                        var exception = new LiveStreamerError(game);
                        Log.Error(string.Format("Could not play stream {0}", game), exception);
                        throw exception;
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
            Log.InfoFormat("Saved Live Streamer configuration at {0}", livestreamerConfig);
        }

        private void MyErrorHandler(AggregateException exception)
        {         
            if (exception.InnerException is LiveStreamerError)
            {
                var message = string.Format("No live feed for {0} available.", exception.InnerException.Message);
                Log.Error(message);
                _dialogService.ShowMessage(message, "Error", "OK");
            }
            _messengerService.Send(new BusyStatusMessage(false, ""), MessengerTokens.ChooseFeedsViewModelToken);
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data)) return;

            if (e.Data.Contains("Starting player"))
            {
                Log.Info("Stream started playing.");
                _messengerService.Send(new BusyStatusMessage(false, "Playing"), MessengerTokens.ChooseFeedsViewModelToken);
            }
            else if (e.Data.Contains("error"))
            {
                var message = string.Format("Could not play stream {0}.", _game);
                Log.Error(message);
                _dialogService.ShowMessage(message, "Error", "OK");
            }
            else if (e.Data.Contains("Stream ended"))
            {                    
                Log.Info("Player closed and stream ended.");                    
            }
            _output.AppendLine(e.Data);
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data)) return;

            _output.AppendLine(e.Data);
        }
    }
}