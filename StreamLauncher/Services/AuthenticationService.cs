using System.Reflection;
using System.Threading.Tasks;
using log4net;
using RestSharp;
using StreamLauncher.Api;
using StreamLauncher.Dtos;
using StreamLauncher.Exceptions;
using StreamLauncher.Models;

namespace StreamLauncher.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHockeyStreamsApiRequiringApiKey _hockeyStreamsApi;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private const string RegularMembership = "REGULAR";

        public AuthenticationService(IHockeyStreamsApiRequiringApiKey hockeyStreamsApi)
        {
            _hockeyStreamsApi = hockeyStreamsApi;            
        }

        public Task<AuthenticationResult> Authenticate(string userName, string password)
        {                        
            try
            {
                var request = new RestRequest { Resource = "Login", Method = Method.POST };                
                request.AddParameter("username", userName, ParameterType.GetOrPost);
                request.AddParameter("password", password, ParameterType.GetOrPost);
                Log.Info("Attempting to authenticate");
                var loginResponseDto = _hockeyStreamsApi.Execute<LoginResponseDto>(request);
                if (loginResponseDto.Membership == RegularMembership)
                {
                    const string errorMessage = "You must have PREMIUM membership to use this app.";
                    Log.InfoFormat("Authentication failed. {0}.", errorMessage);
                    return Task.FromResult(new AuthenticationResult
                    {
                        IsAuthenticated = false,
                        ErrorMessage = errorMessage
                    });                    
                }
                var authenticatedUser = MapLoginResponseDtoToUser(loginResponseDto);
                Log.Info("Authentication successful");
                return Task.FromResult(new AuthenticationResult {IsAuthenticated = true, AuthenticatedUser = authenticatedUser});
            }
            catch (HockeyStreamsApiBadRequest badRequest)
            {
                Log.InfoFormat("Authentication failed. {0}.", badRequest.Message);
                return Task.FromResult(new AuthenticationResult {IsAuthenticated = false, ErrorMessage = badRequest.Message});
            }
        }

        private User MapLoginResponseDtoToUser(LoginResponseDto loginResponseDto)
        {
            return new User
            {
                UserName = loginResponseDto.UserName,
                FavoriteTeam = loginResponseDto.FavTeam,                
                Token = loginResponseDto.Token
            };
        }
    }
}