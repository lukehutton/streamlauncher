﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Interop;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using StreamLauncher.Exceptions;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Messages;
using StreamLauncher.Models;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Util;
using StreamLauncher.Wpf.Infrastructure;
using StreamLauncher.Wpf.Views;

namespace StreamLauncher.Wpf.ViewModel
{
    public class ChooseFeedsViewModel : ViewModelBase, IChooseFeedsViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDialogService _dialogService;
        private readonly IViewModelLocator _viewModelLocator;
        private readonly IHockeyStreamRepository _hockeyStreamRepository;
        private readonly ILiveStreamer _liveStreamer;

        private IEnumerable<Feed> _feeds;
        private string _location;
        private Quality _quality;
        private string _game;
        private string _busyText;
        private bool _isBusy;
        private bool? _dialogResult;

        private bool _isPlaying;
        private string _chooseFeedsTitle;

        public AsyncRelayCommand<int> PlayCommand { get; private set; } 

        public ChooseFeedsViewModel(
            IHockeyStreamRepository hockeyStreamRepository,
            ILiveStreamer liveStreamer,            
            IDialogService dialogService,
            IViewModelLocator viewModelLocator
            )
        {
            _hockeyStreamRepository = hockeyStreamRepository;
            _liveStreamer = liveStreamer;            
            _dialogService = dialogService;
            _viewModelLocator = viewModelLocator;

            PlayCommand = new AsyncRelayCommand<int>(HandlePlayCommand);

            Messenger.Default.Register<BusyStatusMessage>(this, MessengerTokens.ChooseFeedsViewModelToken, HandleBusyStatusMessage);
        }

        public void HandleBusyStatusMessage(BusyStatusMessage busyStatusMessage)
        {
            BusyText = busyStatusMessage.Status;
            IsBusy = busyStatusMessage.IsBusy;

            if (!IsBusy && (BusyText == "Playing" || BusyText.Contains("Error")))
            {
                try
                {
                    DialogResult = false;
                }
                catch (InvalidOperationException ex)
                {
                    Log.Error(ex);
                }
            }
        }

        private Task HandlePlayCommand(int streamId)
        {
            var context = TaskScheduler.FromCurrentSynchronizationContext();            
            return
                Task.Run(
                    () =>
                    {
                        PlayFeed(streamId)
                            .ContinueWith(HandleExceptionWhenPlayingIfAny,
                                context);
                    });
        }

        private void HandleExceptionWhenPlayingIfAny(Task task)
        {
            Exception ex = task.Exception;
            if (ex is AggregateException && ex.InnerException != null)
            {
                ex = ex.InnerException;
                IsBusy = false;
                Log.Error("An exception occurred while playing feed.", ex);
                if (ex is StreamNotFoundException)
                {
                    _dialogService.ShowError(string.Format("Live feed for {0} not found", _game), "Error", "OK");
                }
                else if (ex is HockeyStreamsApiBadRequest)
                {
                    _dialogService.ShowError("You must have PREMIUM membership to use this app.", "Error", "OK");
                }
                else if (ex is LiveStreamerExecutableNotFound)
                {
                    ShowSettingsDialog("Livestreamer Path does not exist.");
                }
                else if (ex is MediaPlayerNotFound)
                {
                    ShowSettingsDialog("Media Player Path does not exist.");
                }
                else if (ex is TimeoutException)
                {
                    _dialogService.ShowError("Timeout exceeded attempting to play feed. Please try again.", "Error", "OK");
                }
                else
                {
                    _dialogService.ShowError("An exception has occurred attempting to play feed. See logs for more details.", "Error", "OK");
                }
            }
        }

        private void ShowSettingsDialog(string errorMessage)
        {
            var settingsViewModel = _viewModelLocator.Settings;
            settingsViewModel.Init();
            settingsViewModel.ErrorMessage = errorMessage;
            _dialogService.ShowDialog<SettingsWindow>(settingsViewModel);
        }

        private async Task PlayFeed(int streamId)
        {            
            BusyText = "Getting feed...";
            IsBusy = true;

            var stream = await _hockeyStreamRepository.GetLiveStream(streamId, _location, _quality);

            IsBusy = false;

            _liveStreamer.Play(_game, stream.Source, _quality);            
        }

        public void Init(IEnumerable<Feed> feeds, string location, Quality quality)
        {            
            Feeds = feeds;
            _game = _feeds.First().Game;
            IsPlaying = _feeds.First().IsPlaying;
            _location = location;
            _quality = quality;
            ChooseFeedsTitle = "Choose Feeds - {0}".Fmt(_game);
        }

        public IEnumerable<Feed> Feeds
        {
            get
            {
                return _feeds;
            }

            set
            {
                _feeds = value;
                RaisePropertyChanged(() => DialogResult);
            }
        }
        
        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }

            set
            {
                _dialogResult = value;
                RaisePropertyChanged(() => DialogResult);
            }
        }

        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                RaisePropertyChanged(() => BusyText);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }     
        
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }    
        
        public string ChooseFeedsTitle
        {
            get { return _chooseFeedsTitle; }
            set
            {
                _chooseFeedsTitle = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }     
    }
}