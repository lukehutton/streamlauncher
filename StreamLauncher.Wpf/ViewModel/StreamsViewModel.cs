using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.Api;
using StreamLauncher.Authentication;
using StreamLauncher.Filters;
using StreamLauncher.Models;
using StreamLauncher.Repositories;

namespace StreamLauncher.Wpf.ViewModel
{
    public class StreamsViewModel : ViewModelBase
    {
        private readonly IHockeyStreamRepository _hockeyStreamRepository;
        private readonly IHockeyStreamFilter _hockeyStreamFilter;

        private readonly IStreamLocationRepository _streamLocationRepository;        
        private readonly ITokenProvider _tokenProvider;
        private readonly IAuthenticationService _authenticationService;

        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;

        private RelayCommand _getLiveStreamsCommand;
        private string _location;
        private string _eventType;

        public StreamsViewModel(IHockeyStreamRepository hockeyStreamRepository,
            IHockeyStreamFilter hockeyStreamFilter,
            IStreamLocationRepository streamLocationRepository,            
            ITokenProvider tokenProvider,
            IAuthenticationService authenticationService
            )
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;
            _streamLocationRepository = streamLocationRepository;            
            _tokenProvider = tokenProvider;
            _authenticationService = authenticationService;

            Streams = new ObservableCollection<HockeyStream>();
            Locations = new ObservableCollection<StreamLocation>();

            AuthenticateUser();

            GetLiveStreams();
            SelectedEventType = "NHL";
            FilterByEventType(SelectedEventType);
            GetLocations();

            SetPreferredLocation();
        }

        private void FilterByEventType(string selectedEventType)
        {
            var selectedEvent = EventTypeParser.Parse(selectedEventType);
            var filteredStreams = _hockeyStreamFilter.By(_hockeyStreams, new EventTypeFilterSpecification(selectedEvent));
            Streams = new ObservableCollection<HockeyStream>(filteredStreams);
        }

        private void AuthenticateUser()
        {
            if (IsInDesignModeStatic) return;

            // todo read from persistance OR load form input
            var userName = Convert.ToString(ConfigurationManager.AppSettings["hockeystreams.userName"]);
            var password = Convert.ToString(ConfigurationManager.AppSettings["hockeystreams.password"]);

            var result = _authenticationService.Authenticate(userName, password);
            _tokenProvider.Token = result.AuthenticatedUser.Token;
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
        public string SelectedEventType
        {
            get { return _eventType; }
            set
            {
                _eventType = value;
                RaisePropertyChanged();
            }
        }

        private async Task GetLiveStreams()
        {
            Streams.Clear();
            var hockeyStreams = await _hockeyStreamRepository.GetLiveStreams(DateTime.Now);
            
            foreach (var hockeyStream in hockeyStreams)
            {
                Streams.Add(hockeyStream);
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

        public RelayCommand GetLiveStreamsCommand
        {
            get
            {
                return _getLiveStreamsCommand
                    ?? (_getLiveStreamsCommand = new RelayCommand(
                        async () =>
                        {
                            await GetLiveStreams();
                        }));
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