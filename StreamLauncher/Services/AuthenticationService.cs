using RestSharp;
using StreamLauncher.Dtos;
using StreamLauncher.Models;
using StreamLauncher.Providers;

namespace StreamLauncher.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHockeyStreamsApiRequiringApiKey _hockeyStreamsApi;

        public AuthenticationService(IHockeyStreamsApiRequiringApiKey hockeyStreamsApi)
        {
            _hockeyStreamsApi = hockeyStreamsApi;            
        }

        public AuthenticationResult Authenticate(string userName, string password)
        {                        
            try
            {
                var request = new RestRequest { Resource = "Login", Method = Method.POST };                
                request.AddParameter("username", userName, ParameterType.GetOrPost);
                request.AddParameter("password", password, ParameterType.GetOrPost);
                var loginResponseDto = _hockeyStreamsApi.Execute<LoginResponseDto>(request);
                var authenticatedUser = MapLoginResponseDtoToUser(loginResponseDto);
                return new AuthenticationResult {IsAuthenticated = true, AuthenticatedUser = authenticatedUser};
            }
            catch (HockeyStreamsApiBadRequest badRequest)
            {
                return new AuthenticationResult {IsAuthenticated = false, ErrorMessage = badRequest.Message};
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