using System.Configuration;

namespace StreamLauncher.Api
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        public string GetApiKey()
        {
            return ConfigurationManager.AppSettings["hockeystreams.apiKey"];            
        }
        public string GetScoresApiKey()
        {
            return ConfigurationManager.AppSettings["hockeystreams.scoresApiKey"];            
        }
    }
}