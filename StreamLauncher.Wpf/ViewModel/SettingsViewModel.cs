using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.Repositories;

namespace StreamLauncher.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IUserSettings _userSettings;
        
        private string _errorMessage;
        private string _liveStreamerPath;
        private string _mediaPlayerPath;

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public SettingsViewModel(IUserSettings userSettings)
        {
            _userSettings = userSettings;

            SaveCommand = new RelayCommand(HandleSaveCommand);
            CancelCommand = new RelayCommand(HandleCancelCommand);
        }

        private void HandleCancelCommand()
        {
            throw new System.NotImplementedException();
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
            _userSettings.Save();
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
    }
}