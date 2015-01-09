using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Repositories;
using StreamLauncher.Validators;

namespace StreamLauncher.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IUserSettings _userSettings;
        private readonly ILiveStreamer _liveStreamer;
        private readonly IUserSettingsValidator _userSettingsValidator;

        private string _busyText;
        private bool _isBusy;

        private bool? _dialogResult;
        private string _errorMessage;
        private string _liveStreamerPath;
        private string _mediaPlayerPath;
        private string _mediaPlayerArguments;

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public SettingsViewModel(IUserSettings userSettings, ILiveStreamer liveStreamer, IUserSettingsValidator userSettingsValidator)
        {
            _userSettings = userSettings;
            _liveStreamer = liveStreamer;
            _userSettingsValidator = userSettingsValidator;

            SaveCommand = new RelayCommand(HandleSaveCommand);
            CancelCommand = new RelayCommand(HandleCancelCommand);
        }

        private void HandleCancelCommand()
        {
            DialogResult = false;
        }

        private void HandleSaveCommand()
        {
            _userSettings.LiveStreamerPath = LiveStreamerPath;
            _userSettings.MediaPlayerPath = MediaPlayerPath;
            _userSettings.MediaPlayerArguments = MediaPlayerArguments;

            var brokenRules =_userSettingsValidator.BrokenRules(_userSettings).ToList();
            if (brokenRules.Any()) 
            { 
                ErrorMessage = brokenRules.First();                
                return;
            }
            _userSettings.Save();

            SaveAsync();            
        }

        private async void SaveAsync()
        {
            BusyText = "Saving settings...";
            IsBusy = true;

            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                _liveStreamer.SaveConfig();
            });

            IsBusy = false;

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

        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }     
    }
}