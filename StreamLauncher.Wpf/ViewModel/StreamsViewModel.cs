using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Api;
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
        private readonly ITokenProvider _tokenProvider;

        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;

        public RelayCommand GetStreamsCommand { get; private set; }        

        private string _location;

        private string _filterEventType;
        private string _filterActiveState;        

        public StreamsViewModel(
            IHockeyStreamRepository hockeyStreamRepository,
            IHockeyStreamFilter hockeyStreamFilter,
            IStreamLocationRepository streamLocationRepository,            
            ITokenProvider tokenProvider
            )
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;
            _streamLocationRepository = streamLocationRepository;            
            _tokenProvider = tokenProvider;

            Streams = new ObservableCollection<HockeyStream>();
            Locations = new ObservableCollection<StreamLocation>();

            GetStreamsCommand = new RelayCommand(GetStreams);            
                        
            Messenger.Default.Register<AuthenticatedMessage>(this, AuthenticationSuccessful);
        }

        private void AuthenticationSuccessful(AuthenticatedMessage authenticatedMessage)
        {            
            _tokenProvider.Token = authenticatedMessage.AuthenticationResult.AuthenticatedUser.Token;

            SelectedFilterEventType = "NHL";
            SelectedFilterActiveState = "All";
            GetStreams();

            GetLocations();
            SetPreferredLocation();            
        }

        private void GetStreams()
        {
            //Task.Run(async () => await GetStreamsFiltered());
            GetStreamsFiltered();
        }

        private void FilterByEventType(string selectedEventType)
        {
            var selectedEvent = EventTypeParser.Parse(selectedEventType);
            var filteredStreams = _hockeyStreamFilter.By(_hockeyStreams, new EventTypeFilterSpecification(selectedEvent));
            Streams = new ObservableCollection<HockeyStream>(filteredStreams);
        }
        private void FilterByActiveState(string selectedActiveState)
        {
            var isPlaying = selectedActiveState == "In progress...";
            if (!isPlaying) return;
            var filteredStreams = _hockeyStreamFilter.By(_hockeyStreams, new ActiveFilterSpecification(true));
            Streams = new ObservableCollection<HockeyStream>(filteredStreams);
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
            }
        }

        public string SelectedFilterActiveState
        {
            get { return _filterActiveState; }
            set
            {
                _filterActiveState = value;
                RaisePropertyChanged();
            }
        }

        private async Task GetStreamsFiltered()
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

            FilterByEventType(SelectedFilterEventType);
            FilterByActiveState(SelectedFilterActiveState);

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