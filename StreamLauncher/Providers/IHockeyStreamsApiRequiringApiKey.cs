using RestSharp;

namespace StreamLauncher.Providers
{
    public interface IHockeyStreamsApiRequiringApiKey
    {
        T Execute<T>(RestRequest request) where T : new();
    }
}