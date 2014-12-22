﻿using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StreamLauncher.Api;
using StreamLauncher.Authentication;
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
        private readonly IAuthenticationService _authenticationService;

        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;

        public RelayCommand GetStreamsCommand { get; private set; }
        public RelayCommand OpenLoginDialogCommand { get; private set; }

        private string _location;

        private string _filterEventType;
        private string _filterActiveState;

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

            GetStreamsCommand = new RelayCommand(GetStreams);
            OpenLoginDialogCommand = new RelayCommand(OpenLoginWindow);

            AuthenticateUser();
            
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

        private void OpenLoginWindow()
        {
            Messenger.Default.Send(new OpenWindowMessage {WindowType = WindowType.Modal});
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