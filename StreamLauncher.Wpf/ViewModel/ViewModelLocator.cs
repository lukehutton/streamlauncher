using Microsoft.Practices.ServiceLocation;
using StreamLauncher.Wpf.StartUp;

namespace StreamLauncher.Wpf.ViewModel
{
    public class ViewModelLocator
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

        public LoginViewModel Login
        {
            get { return ServiceLocator.Current.GetInstance<LoginViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}