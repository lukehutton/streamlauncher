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

        private ObservableCollection<HockeyStream> _hockeyStreams;

        private RelayCommand _getLiveStreamsCommand;

        public StreamsViewModel(IHockeyStreamRepository hockeyStreamRepository, IHockeyStreamFilter hockeyStreamFilter)
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _hockeyStreamFilter = hockeyStreamFilter;

            Streams = new ObservableCollection<HockeyStream>();

            GetLiveStreams();            
        }

        private async Task GetLiveStreams()
        {
            Streams.Clear();
            var hockeyStreams = await _hockeyStreamRepository.GetLiveStreams();
            
            foreach (var hockeyStream in hockeyStreams)
            {
                Streams.Add(hockeyStream);
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
                RaisePropertyChanged("StreamsView");
            }
        }
    }
}