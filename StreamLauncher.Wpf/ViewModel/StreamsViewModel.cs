using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
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

        private readonly object _hockeyStreamsLock = new object();
        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;

        public RelayCommand GetStreamsCommand { get; private set; }        
        public RelayCommand SettingsCommand { get; private set; }        
        public RelayCommand PlayHomeFeedCommand { get; private set; }        
        public RelayCommand PlayAwayFeedCommand { get; private set; }        

        private string _location;
        private string _quality;

        private string _filterEventType;
        private string _filterActiveState;
        private string _favouriteTeam;

        public string FavouriteTeam
        {
            get { return _favouriteTeam.MaxStrLen(AppConstants.MaxTeamStringLength); }
            set
            {
                _favouriteTeam = value;
                RaisePropertyChanged();
            }
        }

        private List<HockeyStream> _allHockeyStreams;
        private bool _isAuthenticated;        

        public StreamsViewModel(
            IHockeyStreamRepository hockeyStreamRepository,
            IHockeyStreamFilter hockeyStreamFilter,
            IStreamLocationRepository streamLocationRepository,
            ILiveStreamer liveStreamer,
            IUserSettings userSettings,
            IDialogService dialogService
            )
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;
            _streamLocationRepository = streamLocationRepository;
            _liveStreamer = liveStreamer;
            _userSettings = userSettings;
            _dialogService = dialogService;

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

        private void HandleSettingsCommand()
        {
            ShowSettingsDialog();
        }

        private void ShowSettingsDialog(string errorMessage = "")
        {
            var settingsViewModel = SimpleIoc.Default.GetInstance<SettingsViewModel>(Guid.NewGuid().ToString());
            settingsViewModel.ErrorMessage = errorMessage;
            settingsViewModel.LiveStreamerPath = _userSettings.LiveStreamerPath;
            settingsViewModel.MediaPlayerPath = _userSettings.MediaPlayerPath;
            settingsViewModel.MediaPlayerArguments = _userSettings.MediaPlayerArguments;
            var settingsWindow = new SettingsWindow
            {
                DataContext = settingsViewModel
            };
            settingsWindow.ShowDialog();
        }

        public HockeyStream SelectedStream { get; set; }

        private void HandlePlayHomeFeedCommand()
        {
            PlayFeed(SelectedStream.HomeStreamId);
        }
        private void HandlePlayAwayFeedCommand()
        {
            PlayFeed(SelectedStream.AwayStreamId);
        }

        private void PlayFeed(int streamId)
        {
            var quality = SelectedQuality == "High Quality (3200Kbps HD)" ? Quality.HD : Quality.SD;

            try
            {
                var stream = _hockeyStreamRepository.GetLiveStream(streamId, SelectedLocation, quality);
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
                MessageBox.Show("You must have PREMIUM membership to use this app.");
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

        private void HandleAuthenticationSuccessfulMessage(AuthenticatedMessage authenticatedMessage)
        {
            SelectedFilterEventType = "ALL";
            SelectedFilterActiveState = "ALL";

            _favouriteTeam = authenticatedMessage.AuthenticationResult.AuthenticatedUser.FavoriteTeam;            
            _isAuthenticated = true;

            GetLocations();
            SetPreferredLocation();
            SetPreferredQuality();

            HandleGetStreamsCommand();
        }

        private async void GetStreamsAsync()
        {
            Messenger.Default.Send(new BusyStatusMessage(true, "Getting streams..."));

            await Task.Run(() => GetStreams());

            Messenger.Default.Send(new BusyStatusMessage(false, ""));
        }

        private void HandleGetStreamsCommand()
        {
            GetStreamsAsync();
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

        private void SetPreferredLocation()
        {
            if (IsInDesignModeStatic)
            {
                SelectedLocation = "Location 2";
            }
            else
            {
                SelectedLocation = "North America - West"; // todo read from persisted settings or set default
            }
        }
        private void SetPreferredQuality()
        {
           SelectedQuality = "High Quality (3200Kbps HD)"; // todo read from persisted settings or set default
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
                if (_isAuthenticated)
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
                if (_isAuthenticated)
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