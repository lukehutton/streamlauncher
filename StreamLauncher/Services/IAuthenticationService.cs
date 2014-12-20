using StreamLauncher.Models;

namespace StreamLauncher.Services
{
    public interface IAuthenticationService
    {
        AuthenticationResult Authenticate(string userName, string password);
    }

    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public string ErrorMessage { get; set; }
        public User AuthenticatedUser { get; set; }
    }
}