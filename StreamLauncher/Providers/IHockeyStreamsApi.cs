using RestSharp;

namespace StreamLauncher.Providers
{
    public interface IHockeyStreamsApi
    {
        T Execute<T>(RestRequest request) where T : new();
    }
}