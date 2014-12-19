using System;
using RestSharp;

namespace StreamLauncher.Providers
{
    public class BaseHockeyStreamsApi
    {     
        const string BaseUrl = "https://api.hockeystreams.com";

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(BaseUrl),                
            };
            
            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var hockeyStreamsException = new ApplicationException(message, response.ErrorException);
                throw hockeyStreamsException;
            }
            return response.Data;
        }
    }

    public class HockeyStreamsApi : BaseHockeyStreamsApi, IHockeyStreamsApi
    {
        
    }

    public class HockeyStreamsApiRequiringToken : BaseHockeyStreamsApi, IHockeyStreamsApiRequiringToken
    {
        private readonly ITokenProvider _tokenProvider;

        public HockeyStreamsApiRequiringToken(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public new T Execute<T>(RestRequest request) where T : new()
        {
            request.AddParameter("token", _tokenProvider.GetAuthenticationToken(), ParameterType.UrlSegment);
            return base.Execute<T>(request);
        }
    }

    public class HockeyStreamsApiRequiringApiKey : BaseHockeyStreamsApi, IHockeyStreamsApiRequiringApiKey
    {
        private readonly IApiKeyProvider _apiKeyProvider;

        public HockeyStreamsApiRequiringApiKey(IApiKeyProvider apiKeyProvider)
        {
            _apiKeyProvider = apiKeyProvider;
        }

        public new T Execute<T>(RestRequest request) where T : new()
        {
            request.AddParameter("key", _apiKeyProvider.GetApiKey(), ParameterType.UrlSegment);
            return base.Execute<T>(request);
        }
    }
}