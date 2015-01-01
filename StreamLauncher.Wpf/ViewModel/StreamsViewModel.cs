using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Filters;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Wpf.Messages;

namespace StreamLauncher.Wpf.ViewModel
{
    public class StreamsViewModel : ViewModelBase
    {
        private readonly IHockeyStreamRepository _hockeyStreamRepository;
        private readonly IHockeyStreamFilter _hockeyStreamFilter;
        private readonly IStreamLocationRepository _streamLocationRepository;                

        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;

        public RelayCommand GetStreamsCommand { get; private set; }        
        public RelayCommand PlayHomeFeedCommand { get; private set; }        

        private string _location;

        private string _filterEventType;
        private string _filterActiveState;
        private string _favouriteTeam;

        private List<HockeyStream> _allHockeyStreams;
        private bool _isAuthenticated;

        public StreamsViewModel(
            IHockeyStreamRepository hockeyStreamRepository,
            IHockeyStreamFilter hockeyStreamFilter,
            IStreamLocationRepository streamLocationRepository                        
            )
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;
            _streamLocationRepository = streamLocationRepository;                        

            Streams = new ObservableCollection<HockeyStream>();
            Locations = new ObservableCollection<StreamLocation>();

            GetStreamsCommand = new RelayCommand(HandleGetStreamsCommand);
            PlayHomeFeedCommand = new RelayCommand(HandlePlayHomeFeedCommand);            
                        
            Messenger.Default.Register<AuthenticatedMessage>(this, HandleAuthenticationSuccessfulMessage);
        }

        public HockeyStream SelectedStream { get; set; }

        private void HandlePlayHomeFeedCommand()
        {
            MessageBox.Show(string.Format("Feed id is {0} for team {1}", SelectedStream.HomeStreamId, SelectedStream.HomeTeam));
        }

        private void HandleAuthenticationSuccessfulMessage(AuthenticatedMessage authenticatedMessage)
        {
            SelectedFilterEventType = "ALL";
            SelectedFilterActiveState = "ALL";

            _favouriteTeam = authenticatedMessage.AuthenticationResult.AuthenticatedUser.FavoriteTeam;
            _isAuthenticated = true;

            GetLocations();
            SetPreferredLocation();            

            HandleGetStreamsCommand();
        }

        private void HandleGetStreamsCommand()
        {
            //Task.Run(async () => await GetStreamsFiltered());
            GetStreams();

            FilterStreams(_allHockeyStreams);
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

        public string SelectedLocation
        {
            get { return _location; }
            set
            {
                _location = value;
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

        private async Task GetStreams()
        {
            Streams.Clear();

            // update status control on top off all controls - see http://msdn.microsoft.com/en-us/magazine/jj694937.aspx
//            Messenger.Default.Send(new StatusMessage("Getting streams...", 0));

            var hockeyStreams = await _hockeyStreamRepository.GetLiveStreams(DateTime.Now);

//            if (hockeyStreams.Count == 0)
//            {
//                DialogService.ShowMessage("We couldn't find any streams.");
//            }

            foreach (var hockeyStream in hockeyStreams)
            {
                Streams.Add(hockeyStream);
            }

            _allHockeyStreams = Streams.ToList();

//            Messenger.Default.Send(new StatusMessage("Done", 3000));
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