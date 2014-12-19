namespace StreamLauncher.Providers
{
    public interface ITokenProvider
    {
        string GetAuthenticationToken();
        void SetAuthenticationToken(string token);
    }
}