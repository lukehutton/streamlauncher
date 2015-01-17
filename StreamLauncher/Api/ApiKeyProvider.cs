using StreamLauncher.Properties;

namespace StreamLauncher.Api
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        public string GetApiKey()
        {
            return Settings.Default.HockeystreamsApiKey;
        }
        public string GetScoresApiKey()
        {
            return Settings.Default.HockeystreamsScoresApiKey;
        }
    }
}