using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using StreamLauncher.Constants;
using StreamLauncher.Filters;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Util;
using StreamLauncher.Wpf.Infrastructure;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Wpf.ViewModel
{
    public class StreamsViewModel : ViewModelBase, IStreamsViewModel
    {
        private readonly IHockeyStreamRepository _hockeyStreamRepository;
        private readonly IHockeyStreamFilter _hockeyStreamFilter;
        private readonly IStreamLocationRepository _streamLocationRepository;        
        private readonly IUserSettings _userSettings;
        private readonly IDialogService _dialogService;
        private readonly IViewModelLocator _viewModelLocator;
        private readonly IMessengerService _messengerService;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _hockeyStreamsLock = new object();
        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;
        private List<HockeyStream> _allHockeyStreams;

        public List<HockeyStream> AllHockeyStreams
        {
            get { return _allHockeyStreams; }
        }

        public AsyncRelayCommand GetStreamsCommand { get; private set; }        
        public RelayCommand SettingsCommand { get; private set; }
        public RelayCommand ChooseFeedsCommand { get; private set; }        

        private string _location;
        private string _quality;

        private string _filterEventType;
        private string _filterActiveState;
        private bool _showScores;

        private string _favouriteTeam;
        private string _showScoresText;
        private HockeyStream _selectedStream;        

        public StreamsViewModel(
            IHockeyStreamRepository hockeyStreamRepository,
            IHockeyStreamFilter hockeyStreamFilter,
            IStreamLocationRepository streamLocationRepository,            
            IUserSettings userSettings,
            IDialogService dialogService,
            IViewModelLocator viewModelLocator,
            IMessengerService messengerService
            )
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;
            _streamLocationRepository = streamLocationRepository;            
            _userSettings = userSettings;
            _dialogService = dialogService;
            _viewModelLocator = viewModelLocator;
            _messengerService = messengerService;

            Streams = new ObservableCollection<HockeyStream>();                        
            Locations = new ObservableCollection<StreamLocation>();

            GetStreamsCommand = new AsyncRelayCommand(HandleGetStreamsCommand);
            SettingsCommand = new RelayCommand(HandleSettingsCommand);
            ChooseFeedsCommand = new RelayCommand(HandleChooseFeedsCommand);            
                        
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
                ShowScoresText = _showScores ? "Scores Off" : "Scores On";
            }
        }          
        
        public string ShowScoresText
        {
            get { return _showScoresText; }
            set
            {
                _showScoresText = value;
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

        public HockeyStream SelectedStream
        {
            get { return _selectedStream; }
            set
            {
                _selectedStream = value;
                RaisePropertyChanged();                
            } 
        }

        private void HandleChooseFeedsCommand()
        {            
            var showFeedsViewModel = _viewModelLocator.ChooseFeeds;
            var quality = SelectedQuality == Qualities.First() ? Quality.HD : Quality.SD;
            showFeedsViewModel.Init(SelectedStream.Feeds, SelectedLocation, quality);         
            _dialogService.ShowDialog<ChooseFeedsWindow>(showFeedsViewModel);
        }      

        public void HandleAuthenticationSuccessfulMessage(AuthenticatedMessage authenticatedMessage)
        {
            SelectedFilterEventType = _userSettings.PreferredEventType.IsNullOrEmpty() ? "NHL" : _userSettings.PreferredEventType;
            SelectedFilterActiveState = "ALL";

            _favouriteTeam = authenticatedMessage.AuthenticationResult.AuthenticatedUser.FavoriteTeam;            
            IsAuthenticated = true;

            GetLocations();
            SelectedLocation = _userSettings.PreferredLocation.IsNullOrEmpty() ? "North America - West" : _userSettings.PreferredLocation;            
            ShowScores = _userSettings.ShowScoring ?? true;

            HandleGetStreamsCommand();            
        }

        private Task HandleGetStreamsCommand()
        {
            _messengerService.Send(new BusyStatusMessage(true, "Getting streams..."), MessengerTokens.MainViewModelToken);

            return Task.Run(() => GetStreams()).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Exception ex = task.Exception;
                    if (ex is AggregateException)
                    {
                        while (ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                            const string message = "An error occurred while getting live streams.";
                            Log.Error(message, ex);
                            _dialogService.ShowError(ex, message, "OK");
                        }
                    }
                }
                else if (!Streams.Any())
                {
                    _dialogService.ShowMessage("We couldn't find any streams.", "Streams not found", "OK");
                }                                 
                _messengerService.Send(new BusyStatusMessage(false, ""), MessengerTokens.MainViewModelToken);
            }, TaskScheduler.FromCurrentSynchronizationContext());              
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

        private async Task GetStreams()
        {
            lock (_hockeyStreamsLock)
            {
                Streams.Clear();
            }
                
            var hockeyStreams = await _hockeyStreamRepository.GetLiveStreams(DateTime.Now);
            var streams = hockeyStreams as IList<HockeyStream> ?? hockeyStreams.ToList();
            if (!streams.Any())
            {
                return;
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