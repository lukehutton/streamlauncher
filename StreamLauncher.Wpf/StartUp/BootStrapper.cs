using System.Net;
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
using StreamLauncher.Util;
using StreamLauncher.Validators;
using StreamLauncher.Wpf.ViewModel;

namespace StreamLauncher.Wpf.StartUp
{
    public class BootStrapper
    {
        public static void Start()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IHockeyStreamRepository, DesignHockeyStreamRepository>();
                SimpleIoc.Default.Register<IStreamLocationRepository, DesignStreamLocationRepository>();
                SimpleIoc.Default.Register<IHockeyStreamFilter, HockeyStreamFilter>();
                SimpleIoc.Default.Register<ITokenProvider, AuthenticationTokenProvider>(); 
                SimpleIoc.Default.Register<IAuthenticationService, AuthenticationService>();
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
                SimpleIoc.Default.Register<IMessengerService, MessengerService>();
                SimpleIoc.Default.Register<IEnvironmentHelper, EnvironmentHelper>();
                SimpleIoc.Default.Register<IThreadSleeper, ThreadSleeper>();
                SimpleIoc.Default.Register<IViewModelLocator, ViewModelLocator>();
                SimpleIoc.Default.Register<IApplicationDispatcher, ApplicationDispatcher>();
                SimpleIoc.Default.Register<IFileHelper, FileHelper>();
                SimpleIoc.Default.Register<IExtractTimeOfDayFromStream, ExtractTimeOfDayFromStream>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<StreamsViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ChooseFeedsViewModel>();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
    }
}