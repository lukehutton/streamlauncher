using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Repositories;

namespace StreamLauncher.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IUserSettings _userSettings;
        private readonly ILiveStreamer _liveStreamer;

        private bool? _dialogResult;
        private string _errorMessage;
        private string _liveStreamerPath;
        private string _mediaPlayerPath;
        private string _mediaPlayerArguments;

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public SettingsViewModel(IUserSettings userSettings, ILiveStreamer liveStreamer)
        {
            _userSettings = userSettings;
            _liveStreamer = liveStreamer;

            SaveCommand = new RelayCommand(HandleSaveCommand);
            CancelCommand = new RelayCommand(HandleCancelCommand);
        }

        private void HandleCancelCommand()
        {
            DialogResult = false;
        }

        private void HandleSaveCommand()
        {
            if (!File.Exists(LiveStreamerPath))
            {
                ErrorMessage = "Livestreamer Path does not exist.";
                return;
            }
            if (!File.Exists(MediaPlayerPath))
            {
                ErrorMessage = "Media Player Path does not exist.";
                return;
            }

            _userSettings.LiveStreamerPath = LiveStreamerPath;
            _userSettings.MediaPlayerPath = MediaPlayerPath;
            _userSettings.MediaPlayerArguments = MediaPlayerArguments;
            _userSettings.Save();

            _liveStreamer.SaveConfig();

            DialogResult = true;
        }

        public string LiveStreamerPath
        {
            get
            {
                return _liveStreamerPath;
            }

            set
            {
                _liveStreamerPath = value;
                RaisePropertyChanged(() => LiveStreamerPath);
            }
        }  

        public string MediaPlayerPath
        {
            get
            {
                return _mediaPlayerPath;
            }

            set
            {
                _mediaPlayerPath = value;
                RaisePropertyChanged(() => MediaPlayerPath);
            }
        }  

        public string MediaPlayerArguments
        {
            get
            {
                return _mediaPlayerArguments;
            }

            set
            {
                _mediaPlayerArguments = value;
                RaisePropertyChanged(() => MediaPlayerArguments);
            }
        }  

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }

            set
            {
                _dialogResult = value;
                RaisePropertyChanged(() => DialogResult);
            }
        }
    }
}