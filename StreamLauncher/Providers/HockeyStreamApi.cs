using System;
using RestSharp;

namespace StreamLauncher.Providers
{
    // todo api requiring key and token
    // i.e. request.AddParameter("key", _key, ParameterType.UrlSegment);
    //      request.AddParameter("token", _token, ParameterType.UrlSegment);
    public class HockeyStreamsApi : IHockeyStreamsApi
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
}