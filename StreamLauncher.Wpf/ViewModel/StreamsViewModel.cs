using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

        private ObservableCollection<HockeyStream> _hockeyStreams;
        private ObservableCollection<StreamLocation> _streamLocations;

        private RelayCommand _getLiveStreamsCommand;
        private string _location;

        public StreamsViewModel(IHockeyStreamRepository hockeyStreamRepository, 
            IHockeyStreamFilter hockeyStreamFilter,
            IStreamLocationRepository streamLocationRepository)
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;
            _streamLocationRepository = streamLocationRepository;

            Streams = new ObservableCollection<HockeyStream>();
            Locations = new ObservableCollection<StreamLocation>();

            GetLiveStreams();
            GetLocations();

            SetPreferredLocation();
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