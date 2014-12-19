using RestSharp;

namespace StreamLauncher.Providers
{
    public interface IHockeyStreamsApiRequiringToken
    {
        T Execute<T>(RestRequest request) where T : new();
    }
}