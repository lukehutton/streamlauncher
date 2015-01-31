using System.Collections.Generic;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Wpf.Infrastructure;

namespace StreamLauncher.Wpf.ViewModel
{
    public interface IChooseFeedsViewModel
    {
        AsyncRelayCommand<int> PlayCommand { get; }
        IEnumerable<Feed> Feeds { get; set; }
        bool? DialogResult { get; set; }
        string BusyText { get; set; }
        bool IsBusy { get; set; }
        void Init(IEnumerable<Feed> feeds, string location, Quality quality);
    }
}