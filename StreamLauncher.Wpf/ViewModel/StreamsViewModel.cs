using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Constants;
using StreamLauncher.Exceptions;
using StreamLauncher.Filters;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Util;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Wpf.ViewModel
{
    public class StreamsViewModel : ViewModelBase
    {
        private readonly IHockeyStreamRepository _hockeyStreamRepository;
        private readonly IHockeyStreamFilter _hockeyStreamFilter;
        private readonly IStreamLocationRepository _streamLocationRepository;
        private readonly ILiveStreamer _liveStreamer;
        private readonly IUserSettings _userSettings;
        private readonly IDialogService _dialogService;
        private readonly IViewModelLocator _viewModelLocator;
        private readonly IMessengerService _messengerService;

        private readonly object _hockeyStreamsLock = new object();
        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;
        private List<HockeyStream> _allHockeyStreams;

        public RelayCommand GetStreamsCommand { get; private set; }        
        public RelayCommand SettingsCommand { get; private set; }        
        public RelayCommand PlayHomeFeedCommand { get; private set; }        
        public RelayCommand PlayAwayFeedCommand { get; private set; }        

        private string _location;
        private string _quality;

        private string _filterEventType;
        private string _filterActiveState;
        private bool _showScores;

        private string _favouriteTeam;        

        public StreamsViewModel(
            IHockeyStreamRepository hockeyStreamRepository,
            IHockeyStreamFilter hockeyStreamFilter,
            IStreamLocationRepository streamLocationRepository,
            ILiveStreamer liveStreamer,
            IUserSettings userSettings,
            IDialogService dialogService,
            IViewModelLocator viewModelLocator,
            IMessengerService messengerService
            )
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;
            _streamLocationRepository = streamLocationRepository;
            _liveStreamer = liveStreamer;
            _userSettings = userSettings;
            _dialogService = dialogService;
            _viewModelLocator = viewModelLocator;
            _messengerService = messengerService;

            Streams = new ObservableCollection<HockeyStream>();                        
            Locations = new ObservableCollection<StreamLocation>();

            GetStreamsCommand = new RelayCommand(HandleGetStreamsCommand);
            SettingsCommand = new RelayCommand(HandleSettingsCommand);
            PlayHomeFeedCommand = new RelayCommand(HandlePlayHomeFeedCommand);            
            PlayAwayFeedCommand = new RelayCommand(HandlePlayAwayFeedCommand);            
                        
            Messenger.Default.Register<AuthenticatedMessage>(this, HandleAuthenticationSuccessfulMessage);
            BindingOperations.CollectionRegistering += BindingOperations_CollectionRegistering;
        }

        private void BindingOperations_CollectionRegistering(object sender, CollectionRegisteringEventArgs e)
        {
            if (e.Collection.Equals(Streams))
            {
                BindingOperations.EnableCollectionSynchronization(Streams, _hockeyStreamsLock);
            }            
        }

        public bool ShowScores
        {
            get { return _showScores; }
            set
            {
                _showScores = value;
                RaisePropertyChanged();
            }
        }   

        private void HandleSettingsCommand()
        {
            ShowSettingsDialog();
        }

        private void ShowSettingsDialog(string errorMessage = "")
        {
            var settingsViewModel = _viewModelLocator.Settings;            
            settingsViewModel.Init();
            settingsViewModel.ErrorMessage = errorMessage;
            _dialogService.ShowDialog<SettingsWindow>(settingsViewModel);
        }

        public HockeyStream SelectedStream { get; set; }

        public void HandlePlayHomeFeedCommand()
        {
            Task.Run(() => PlayFeed(SelectedStream.HomeStreamId));
        }

        public void HandlePlayAwayFeedCommand()
        {
            Task.Run(() => PlayFeed(SelectedStream.AwayStreamId));
        }

        private async void PlayFeed(int streamId)
        {            
            try
            {
                var quality = SelectedQuality == "High Quality (3200Kbps HD)" ? Quality.HD : Quality.SD;

                Messenger.Default.Send(new BusyStatusMessage(true, "Getting stream..."));

                var stream = await _hockeyStreamRepository.GetLiveStream(streamId, SelectedLocation, quality);

                Messenger.Default.Send(new BusyStatusMessage(false, ""));

                var game = string.Format("{0} at {1}", SelectedStream.AwayTeam, SelectedStream.HomeTeam);
                _liveStreamer.Play(game, stream.Source, quality);
            }
            catch (StreamNotFoundException)
            {
                _dialogService.ShowError(string.Format("Live feed for {0} at {1} not found",
                    SelectedStream.AwayTeam,
                    SelectedStream.HomeTeam), "Error", "OK");
            }
            catch (HockeyStreamsApiBadRequest)
            {
                _dialogService.ShowError("You must have PREMIUM membership to use this app.", "Error", "OK");
            }
            catch (LiveStreamerExecutableNotFound)
            {
                ShowSettingsDialog("Livestreamer Path does not exist.");
            }
            catch (MediaPlayerNotFound)
            {
                ShowSettingsDialog("Media Player Path does not exist.");
            }
        }

        public void HandleAuthenticationSuccessfulMessage(AuthenticatedMessage authenticatedMessage)
        {
            SelectedFilterEventType = _userSettings.PreferredEventType.IsNullOrEmpty() ? "NHL" : _userSettings.PreferredEventType;
            SelectedFilterActiveState = "ALL";

            _favouriteTeam = authenticatedMessage.AuthenticationResult.AuthenticatedUser.FavoriteTeam;            
            IsAuthenticated = true;

            GetLocations();
            SelectedLocation = _userSettings.PreferredLocation.IsNullOrEmpty() ? "North America - West" : _userSettings.PreferredLocation;

            HandleGetStreamsCommand();            
        }

        private async void HandleGetStreamsCommand()
        {
            _messengerService.Send(new BusyStatusMessage(true, "Getting streams..."));

            await Task.Run(() => GetStreams());

            _messengerService.Send(new BusyStatusMessage(false, ""));
        }

        private void FilterStreams(IEnumerable<HockeyStream> streams)
        {
            var filteredStreams = FilterByEventType(streams, SelectedFilterEventType);
            filteredStreams = FilterByActiveState(filteredStreams, SelectedFilterActiveState);
            Streams = new ObservableCollection<HockeyStream>(filteredStreams);            
        }

        private IEnumerable<HockeyStream> FilterByEventType(IEnumerable<HockeyStream> streams, string selectedEventType)
        {
            if (selectedEventType == "ALL") return streams;
            var selectedEvent = EventTypeParser.Parse(selectedEventType);
            return _hockeyStreamFilter.By(streams.ToList(), new EventTypeFilterSpecification(selectedEvent));
        }

        public List<string> EventTypes
        {
            get
            {
                return new List<string>
                {
                    "ALL",
                    "AHL",
                    "NHL",
                    "OHL",
                    "QMJHL",
                    "WHL",
                    "World Juniors"
                };
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

        private IEnumerable<HockeyStream> FilterByActiveState(IEnumerable<HockeyStream> streams, string selectedActiveState)
        {
            if (selectedActiveState == "ALL") return streams;
            return _hockeyStreamFilter.By(streams.ToList(), new ActiveFilterSpecification(selectedActiveState));
        }

        public List<string> ActiveStates
        {
            get
            {
                return new List<string>
                {
                    "ALL",
                    "In Progress",
                    "Coming Soon",
                    "Completed"
                };
            }
        }

        public string SelectedLocation
        {
            get { return _location; }
            set
            {
                _location = value;
                RaisePropertyChanged();
            }
        }       
        
        public string SelectedQuality
        {
            get { return _quality; }
            set
            {
                _quality = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedFilterEventType
        {
            get { return _filterEventType; }
            set
            {                
                _filterEventType = value;                
                RaisePropertyChanged();
                if (IsAuthenticated)
                {
                    FilterStreams(_allHockeyStreams);
                }
            }
        }

        public string SelectedFilterActiveState
        {
            get { return _filterActiveState; }
            set
            {
                _filterActiveState = value;
                RaisePropertyChanged();
                if (IsAuthenticated)
                {
                    FilterStreams(_allHockeyStreams);
                }                
            }
        }

        private async void GetStreams()
        {
            lock (_hockeyStreamsLock)
            {
                Streams.Clear();
            }

            var hockeyStreams = await _hockeyStreamRepository.GetLiveStreams(DateTime.Now);
            var streams = hockeyStreams as IList<HockeyStream> ?? hockeyStreams.ToList();
            if (!streams.Any())
            {
                _dialogService.ShowMessage("We couldn't find any streams.", "Error", "OK");
            }

            foreach (var hockeyStream in streams)
            {
                if (hockeyStream.HomeTeam == FavouriteTeam)
                {
                    hockeyStream.IsFavorite = true;
                    hockeyStream.HomeTeam = "*" + hockeyStream.HomeTeam;
                }
                else if (hockeyStream.AwayTeam == FavouriteTeam)
                {
                    hockeyStream.IsFavorite = true;
                    hockeyStream.AwayTeam = "*" + hockeyStream.AwayTeam;
                }

                lock (_hockeyStreamsLock)
                {
                    Streams.Add(hockeyStream);
                }
            }

            _allHockeyStreams = Streams.ToList();

            FilterStreams(_allHockeyStreams);
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

        public string FavouriteTeam
        {
            get { return _favouriteTeam.MaxStrLen(AppConstants.MaxTeamStringLength); }
            set
            {
                _favouriteTeam = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAuthenticated { get; private set; }

        public ObservableCollection<HockeyStream> Streams
        {
            get { return _hockeyStreams; }
            set
            {
                _hockeyStreams = value;
                RaisePropertyChanged();
            }
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
    }
}