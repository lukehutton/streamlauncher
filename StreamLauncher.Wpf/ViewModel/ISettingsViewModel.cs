using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.Models;

namespace StreamLauncher.Wpf.ViewModel
{
    public interface ISettingsViewModel
    {
        RelayCommand SaveCommand { get; }
        RelayCommand CancelCommand { get; }
        string PreferredQuality { get; set; }
        List<string> Qualities { get; }
        bool ShowScoring { get; set; }
        int RtmpTimeOutInSeconds { get; set; }
        string PreferredEventType { get; set; }
        string PreferredLocation { get; set; }
        ObservableCollection<StreamLocation> Locations { get; set; }
        List<string> EventTypes { get; }
        string LiveStreamerPath { get; set; }
        string MediaPlayerPath { get; set; }
        string MediaPlayerArguments { get; set; }
        string ErrorMessage { get; set; }
        bool? DialogResult { get; set; }
        string BusyText { get; set; }
        bool IsBusy { get; set; }
        void Init();
    }
}