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
using StreamLauncher.Api;
using StreamLauncher.Design;
using StreamLauncher.Filters;
using StreamLauncher.Mappers;
using StreamLauncher.MediaPlayers;
using StreamLauncher.Repositories;
using StreamLauncher.Services;
using StreamLauncher.Validators;

namespace StreamLauncher.Wpf.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IHockeyStreamRepository, DesignHockeyStreamRepository>();
                SimpleIoc.Default.Register<IStreamLocationRepository, DesignStreamLocationRepository>();
                SimpleIoc.Default.Register<IHockeyStreamFilter, HockeyStreamFilter>();
                SimpleIoc.Default.Register<ITokenProvider, AuthenticationTokenProvider>(); //todo make design
                SimpleIoc.Default.Register<IAuthenticationService, AuthenticationService>(); //todo make design
            }
            else
            {             
                SimpleIoc.Default.Register<ITokenProvider, AuthenticationTokenProvider>();
                SimpleIoc.Default.Register<IApiKeyProvider, ApiKeyProvider>();
                SimpleIoc.Default.Register<IHockeyStreamsApi, HockeyStreamsApi>();
                SimpleIoc.Default.Register<IHockeyStreamsApiRequiringApiKey, HockeyStreamsApiRequiringApiKey>();
                SimpleIoc.Default.Register<IHockeyStreamsApiRequiringScoresApiKey, HockeyStreamsApiRequiringScoresApiKey>();
                SimpleIoc.Default.Register<IHockeyStreamsApiRequiringToken, HockeyStreamsApiRequiringToken>();                
                SimpleIoc.Default.Register<ILiveStreamScheduleAggregatorAndMapper, LiveStreamScheduleAggregatorAndMapper>();
                SimpleIoc.Default.Register<IStreamLocationRepository, StreamLocationRepository>();
                SimpleIoc.Default.Register<IScoresRepository, ScoresRepository>();
                SimpleIoc.Default.Register<IHockeyStreamRepository, HockeyStreamRepository>();
                SimpleIoc.Default.Register<IHockeyStreamFilter, HockeyStreamFilter>();
                SimpleIoc.Default.Register<IAuthenticationService, AuthenticationService>();
                SimpleIoc.Default.Register<IUserSettings, UserSettings>();
                SimpleIoc.Default.Register<IUserSettingsValidator, UserSettingsValidator>();
                SimpleIoc.Default.Register<ILiveStreamer, LiveStreamer>();
                SimpleIoc.Default.Register<IDialogService, DialogService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<StreamsViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
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
        public LoginViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}