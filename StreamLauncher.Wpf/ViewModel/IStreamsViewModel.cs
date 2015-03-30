using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Wpf.Infrastructure;

namespace StreamLauncher.Wpf.ViewModel
{
    public interface IStreamsViewModel
    {
        AsyncRelayCommand GetStreamsCommand { get; }
        RelayCommand SettingsCommand { get; }
        RelayCommand ChooseFeedsCommand { get; }        
        bool ShowScores { get; set; }
        string ShowScoresText { get; set; }
        HockeyStream SelectedStream { get; set; }
        List<string> EventTypes { get; }
        List<string> Qualities { get; }
        List<string> ActiveStates { get; }
        string SelectedLocation { get; set; }
        string SelectedQuality { get; set; }
        string SelectedFilterEventType { get; set; }
        string SelectedFilterActiveState { get; set; }
        string FavouriteTeam { get; set; }
        bool IsAuthenticated { get; }
        ObservableCollection<HockeyStream> Streams { get; set; }
        ObservableCollection<StreamLocation> Locations { get; set; }
        List<HockeyStream> AllHockeyStreams { get; }
        void HandleAuthenticationSuccessfulMessage(AuthenticatedMessage authenticatedMessage);
        void HandleUserSettingsUpdatedMessage(UserSettingsUpdated userSettingsUpdated);
    }
}