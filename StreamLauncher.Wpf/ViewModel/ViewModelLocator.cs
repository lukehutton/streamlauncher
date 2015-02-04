using System;
using Microsoft.Practices.ServiceLocation;
using StreamLauncher.Wpf.StartUp;

namespace StreamLauncher.Wpf.ViewModel
{
    public class ViewModelLocator : IViewModelLocator
    {
        public ViewModelLocator()
        {
            BootStrapper.Start();
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public StreamsViewModel Streams
        {
            get { return ServiceLocator.Current.GetInstance<StreamsViewModel>(); }
        }

        public ILoginViewModel Login
        {
            get { return ServiceLocator.Current.GetInstance<LoginViewModel>(Guid.NewGuid().ToString()); }
        }

        public ISettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(Guid.NewGuid().ToString()); }
        }

        public IChooseFeedsViewModel ChooseFeeds
        {
            get { return ServiceLocator.Current.GetInstance<ChooseFeedsViewModel>(Guid.NewGuid().ToString()); }            
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}