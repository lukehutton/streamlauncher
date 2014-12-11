using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using StreamLauncher.Filters;
using StreamLauncher.Models;
using StreamLauncher.Repositories;

namespace StreamLauncher.Wpf.ViewModel
{
    public class LiveStreamsViewModel : ViewModelBase
    {
        private readonly IHockeyStreamRepository _hockeyStreamRepository;
        private readonly IHockeyStreamFilter _hockeyStreamFilter;

        private ObservableCollection<HockeyStream> _hockeyStreams;

        public LiveStreamsViewModel(IHockeyStreamRepository hockeyStreamRepository, IHockeyStreamFilter hockeyStreamFilter)
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;

            LiveStreams = new ObservableCollection<HockeyStream>(_hockeyStreamRepository.GetLiveStreams());
        }

        public ObservableCollection<HockeyStream> LiveStreams
        {
            get { return _hockeyStreams; }
            set
            {
                _hockeyStreams = value;
                RaisePropertyChanged("LiveStreams");
            }
        }
    }
}