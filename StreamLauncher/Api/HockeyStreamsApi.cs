﻿using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;
using RestSharp;
using RestSharp.Deserializers;
using StreamLauncher.Dtos;
using StreamLauncher.Exceptions;
using StreamLauncher.Util;

namespace StreamLauncher.Api
{
    public class BaseHockeyStreamsApi
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int ApiTimeOutInSecs = 10;

        const string BaseUrl = "https://api.hockeystreams.com";

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient {BaseUrl = new Uri(BaseUrl), Timeout = ApiTimeOutInSecs*1000};

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            LogRequest(request, client);
            var response = Retry.Do(() => RestResponse<T>(request, client), TimeSpan.FromSeconds(1));            
            LogResponse(response);

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

        private static IRestResponse<T> RestResponse<T>(RestRequest request, RestClient client) where T : new()
        {
            return client.Execute<T>(request);
        }

        private static void LogResponse<T>(IRestResponse<T> response) where T : new()
        {
            Log.Info(string.Format("Response status: {0}, Response content: {1}", response.StatusCode, response.Content));
        }

        private static void LogRequest(IRestRequest request, IRestClient client)
        {
            var parameters = new StringBuilder();
            foreach (var parameter in request.Parameters)
            {
                if (parameter.Name.ToLower() == "password")
                {
                    parameters.AppendFormat("password={0}&", "*".Repeat(Convert.ToString(parameter.Value).Length));
                }
                else
                {
                    parameters.Append(parameter + "&");
                }
            }
            Log.InfoFormat("Request {0}{1}{2}{3}", client.BaseUrl, request.Resource,
                request.Parameters.Any() ? "?" : string.Empty, parameters.ToString().TrimEnd('&'));
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