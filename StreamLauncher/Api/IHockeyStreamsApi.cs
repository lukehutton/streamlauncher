using RestSharp;

namespace StreamLauncher.Api
{
    public interface IHockeyStreamsApi
    {
        T Execute<T>(RestRequest request) where T : new();
    }

    public interface IHockeyStreamsApiRequiringApiKey
    {
        T Execute<T>(RestRequest request) where T : new();
    }
    public interface IHockeyStreamsApiRequiringScoresApiKey
    {
        T Execute<T>(RestRequest request) where T : new();
    }

    public interface IHockeyStreamsApiRequiringToken
    {
        T Execute<T>(RestRequest request) where T : new();
    }
}