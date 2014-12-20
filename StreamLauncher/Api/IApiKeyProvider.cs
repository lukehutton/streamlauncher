namespace StreamLauncher.Api
{
    public interface IApiKeyProvider
    {
        string GetApiKey();
        string GetScoresApiKey();
    }
}