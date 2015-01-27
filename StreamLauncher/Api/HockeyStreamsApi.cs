using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;
using RestSharp;
using RestSharp.Deserializers;
using StreamLauncher.Dtos;
using StreamLauncher.Exceptions;

namespace StreamLauncher.Api
{
    public class BaseHockeyStreamsApi
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const string BaseUrl = "https://api.hockeystreams.com";

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient { BaseUrl = new Uri(BaseUrl) };            

            var parameters = new StringBuilder();
            foreach (var parameter in request.Parameters.Where(parameter => parameter.Name != "password" && parameter.Name != "token"))
            {
                parameters.Append(parameter + "&");
            }            
            Log.InfoFormat("Request {0}/{1}?{2}", client.BaseUrl, request.Resource, parameters.ToString().TrimEnd('&'));
            
            var response = client.Execute<T>(request);
            
            Log.Info(string.Format("Response status: {0}, Response content: {1}", response.StatusCode, response.Content));

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var error = new JsonDeserializer().Deserialize<ErrorResponseDto>(response);
                throw new HockeyStreamsApiBadRequest(error.Msg);
            }            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException(string.Format("Error. Api returned {0} status", response.StatusCode));
            }
            return response.Data; // 200 OK
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
            request.AddParameter("token", _tokenProvider.Token, ParameterType.GetOrPost);
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
            request.AddParameter("key", _apiKeyProvider.GetApiKey(), ParameterType.GetOrPost);
            return base.Execute<T>(request);
        }
    }
    public class HockeyStreamsApiRequiringScoresApiKey : BaseHockeyStreamsApi, IHockeyStreamsApiRequiringScoresApiKey
    {
        private readonly IApiKeyProvider _apiKeyProvider;

        public HockeyStreamsApiRequiringScoresApiKey(IApiKeyProvider apiKeyProvider)
        {
            _apiKeyProvider = apiKeyProvider;
        }

        public new T Execute<T>(RestRequest request) where T : new()
        {
            request.AddParameter("key", _apiKeyProvider.GetScoresApiKey(), ParameterType.GetOrPost);
            return base.Execute<T>(request);
        }
    }
}