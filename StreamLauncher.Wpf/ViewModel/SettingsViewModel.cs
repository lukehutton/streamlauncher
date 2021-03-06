﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Validators;
using StreamLauncher.Util;
using StreamLauncher.Wpf.Infrastructure;

namespace StreamLauncher.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private const string DefaultEventType = "NHL";
        private const string DefaultLocation = "North America - West";
        private const int DefaultRtmpTimeOutInSeconds = 5;
        private const int DefaultRefreshStreamsIntervalInMinutes = 2;

        private readonly IUserSettings _userSettings;
        private readonly ILiveStreamer _liveStreamer;
        private readonly IUserSettingsValidator _userSettingsValidator;
        private readonly IStreamLocationRepository _streamLocationRepository;
        private readonly IEnvironmentHelper _environmentHelper;
        private readonly IThreadSleeper _threadSleeper;
        private readonly IMessengerService _messengerService;

        private string _busyText;
        private bool _isBusy;

        private bool? _dialogResult;
        private string _errorMessage;
        private string _liveStreamerPath;
        private string _mediaPlayerPath;
        private string _mediaPlayerArguments;
        private ObservableCollection<StreamLocation> _streamLocations;
        private string _preferredLocation;
        private string _preferredEventType;
        private int _rtmpTimeOutInSeconds;
        private bool _showScoring;
        private string _preferredQuality;
        private bool _refreshStreamsEnabled;
        private int _refreshStreamsIntervalInMinutes;

        public AsyncRelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public SettingsViewModel(
            IUserSettings userSettings, 
            ILiveStreamer liveStreamer, 
            IUserSettingsValidator userSettingsValidator, 
            IStreamLocationRepository streamLocationRepository,
            IEnvironmentHelper environmentHelper,
            IThreadSleeper threadSleeper, 
            IMessengerService messengerService)
        {
            _userSettings = userSettings;
            _liveStreamer = liveStreamer;
            _userSettingsValidator = userSettingsValidator;
            _streamLocationRepository = streamLocationRepository;
            _environmentHelper = environmentHelper;
            _threadSleeper = threadSleeper;
            _messengerService = messengerService;

            Locations = new ObservableCollection<StreamLocation>();

            SaveCommand = new AsyncRelayCommand(HandleSaveCommand);
            CancelCommand = new RelayCommand(HandleCancelCommand);                    
        }

        public void Init()
        {
            GetLocations();

            if (_userSettings.LiveStreamerPath.IsNullOrEmpty())
            {
                LiveStreamerPath = _environmentHelper.Is64BitEnvironment()
                    ? LiveStreamer.Default64BitLocation
                    : LiveStreamer.Default32BitLocation;
            }
            else
            {
                LiveStreamerPath = _userSettings.LiveStreamerPath;
            }

            if (_userSettings.MediaPlayerPath.IsNullOrEmpty())
            {
                MediaPlayerPath = _environmentHelper.Is64BitEnvironment()
                    ? Vlc.Default64BitLocation
                    : Vlc.Default32BitLocation;
            }
            else
            {
                MediaPlayerPath = _userSettings.MediaPlayerPath;
            }

            MediaPlayerArguments = _userSettings.MediaPlayerArguments ?? Vlc.DefaultArgs;

            PreferredEventType = _userSettings.PreferredEventType.IsNullOrEmpty() ? DefaultEventType : _userSettings.PreferredEventType;
            PreferredLocation = _userSettings.PreferredLocation.IsNullOrEmpty() ? DefaultLocation : _userSettings.PreferredLocation;            
            RtmpTimeOutInSeconds = _userSettings.RtmpTimeOutInSeconds ?? DefaultRtmpTimeOutInSeconds;            
            ShowScoring = _userSettings.ShowScoring ?? true;            
            PreferredQuality = _userSettings.PreferredQuality.IsNullOrEmpty() ? Qualities.First() : _userSettings.PreferredQuality;
            RefreshStreamsEnabled = _userSettings.RefreshStreamsEnabled ?? true;
            RefreshStreamsIntervalInMinutes = _userSettings.RefreshStreamsIntervalInMinutes ?? DefaultRefreshStreamsIntervalInMinutes;
        }

        public int RefreshStreamsIntervalInMinutes
        {
            get { return _refreshStreamsIntervalInMinutes; }
            set
            {
                _refreshStreamsIntervalInMinutes = value;
                RaisePropertyChanged();
            }
        }

        public bool RefreshStreamsEnabled
        {
            get { return _refreshStreamsEnabled; }
            set
            {
                _refreshStreamsEnabled = value;
                RaisePropertyChanged();
            }
        }

        public string PreferredQuality
        {
            get { return _preferredQuality; }
            set
            {
                _preferredQuality = value;
                RaisePropertyChanged();
            }
        }   

        public List<string> Qualities
        {
            get
            {
                return new List<string>
                {
                    "High Quality (3200Kbps HD)",
                    "Low Quality (800Kbps SD)"
                };
            }
        }

        public bool ShowScoring
        {
            get { return _showScoring; }
            set
            {
                _showScoring = value;
                RaisePropertyChanged();
            }
        }   

        public int RtmpTimeOutInSeconds
        {
            get { return _rtmpTimeOutInSeconds; }
            set
            {
                _rtmpTimeOutInSeconds = value;
                RaisePropertyChanged();
            }
        }   

        public string PreferredEventType
        {
            get { return _preferredEventType; }
            set
            {
                _preferredEventType = value;
                RaisePropertyChanged();
            }
        }   

        public string PreferredLocation
        {
            get { return _preferredLocation; }
            set
            {
                _preferredLocation = value;
                RaisePropertyChanged();
            }
        }   

        private void HandleCancelCommand()
        {
            DialogResult = false;
        }

        private async Task HandleSaveCommand()
        {
            _userSettings.LiveStreamerPath = LiveStreamerPath;
            _userSettings.MediaPlayerPath = MediaPlayerPath;
            _userSettings.MediaPlayerArguments = MediaPlayerArguments;
            _userSettings.PreferredEventType = PreferredEventType;
            _userSettings.PreferredLocation = PreferredLocation;
            _userSettings.RtmpTimeOutInSeconds = RtmpTimeOutInSeconds;
            _userSettings.ShowScoring = ShowScoring;
            _userSettings.RefreshStreamsEnabled = RefreshStreamsEnabled;
            _userSettings.RefreshStreamsIntervalInMinutes = RefreshStreamsIntervalInMinutes;

            var brokenRules =_userSettingsValidator.BrokenRules(_userSettings).ToList();
            if (brokenRules.Any()) 
            { 
                ErrorMessage = brokenRules.First();                
                return;
            }

            await SaveSettingsAndLiveStreamerConfigAsync();

            _messengerService.Send(new UserSettingsUpdated(), MessengerTokens.StreamsViewModelToken);
        }

        private async Task SaveSettingsAndLiveStreamerConfigAsync()
        {
            BusyText = "Saving settings...";
            IsBusy = true;

            await Task.Run(() =>
            {
                _threadSleeper.SleepFor(1);
                _userSettings.Save();
                _liveStreamer.SaveConfig();
            });

            IsBusy = false;
            DialogResult = true;
        }

        public ObservableCollection<StreamLocation> Locations
        {
            get { return _streamLocations; }
            set
            {
                _streamLocations = value;
                RaisePropertyChanged();
            }
        }

        private void GetLocations()
        {
            Locations.Clear();
            var locations = _streamLocationRepository.GetLocations();

            foreach (var location in locations)
            {
                Locations.Add(location);
            }
        }

        public List<string> EventTypes
        {
            get
            {
                return new List<string>
                {         
                    "AHL",
                    "NHL",
                    "OHL",
                    "QMJHL",
                    "WHL",
                    "World Juniors"
                };
            }
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
                RaisePropertyChanged(() => BusyText);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }     
    }
}