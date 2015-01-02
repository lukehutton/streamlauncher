using RestSharp;
using StreamLauncher.Api;
using StreamLauncher.Dtos;
using StreamLauncher.Exceptions;
using StreamLauncher.Models;

namespace StreamLauncher.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHockeyStreamsApiRequiringApiKey _hockeyStreamsApi;
        private const string RegularMembership = "REGULAR";

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
                if (loginResponseDto.Membership == RegularMembership)
                {
                    return new AuthenticationResult
                    {
                        IsAuthenticated = false,
                        ErrorMessage = "You must have PREMIUM membership to use this app."
                    };                    
                }

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