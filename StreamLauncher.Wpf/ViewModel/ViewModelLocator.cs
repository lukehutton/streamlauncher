/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:StreamLauncher.WpfApplication"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using StreamLauncher.Filters;
using StreamLauncher.Providers;
using StreamLauncher.Repositories;

namespace StreamLauncher.Wpf.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models                
                SimpleIoc.Default.Register<IHockeyStreamRepository, InMemoryHockeyStreamRepository>();
                SimpleIoc.Default.Register<IStreamLocationRepository, InMemoryStreamLocationRepository>();
                SimpleIoc.Default.Register<IHockeyStreamFilter, HockeyStreamFilter>();
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IHockeyStreamsApi, HockeyStreamsApi>();
                SimpleIoc.Default.Register<IHockeyStreamRepository, InMemoryHockeyStreamRepository>();
                SimpleIoc.Default.Register<IStreamLocationRepository, StreamLocationRepository>();
                SimpleIoc.Default.Register<IHockeyStreamFilter, HockeyStreamFilter>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<StreamsViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public StreamsViewModel Streams
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StreamsViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}