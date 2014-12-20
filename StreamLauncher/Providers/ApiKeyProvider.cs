using System.Configuration;

namespace StreamLauncher.Providers
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        public string GetApiKey()
        {
            return ConfigurationManager.AppSettings["hockeystreams.apiKey"];            
        }
    }
}