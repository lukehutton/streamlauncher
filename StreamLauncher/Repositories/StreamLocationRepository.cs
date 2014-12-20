using System.Collections.Generic;
using RestSharp;
using StreamLauncher.Dtos;
using StreamLauncher.Models;
using System.Linq;
using StreamLauncher.Api;

namespace StreamLauncher.Repositories
{
    public class StreamLocationRepository : IStreamLocationRepository
    {
        private readonly IHockeyStreamsApi _hockeyStreamsApi;

        public StreamLocationRepository(IHockeyStreamsApi hockeyStreamsApi)
        {
            _hockeyStreamsApi = hockeyStreamsApi;
        }

        public IEnumerable<StreamLocation> GetLocations()
        {
            // todo cache results
            // todo handle exception and failed exception by caller            
            var request = new RestRequest {Resource = "GetLocations"};
            var locations = _hockeyStreamsApi.Execute<List<GetLocationsResponseDto>>(request);                        
            return MapLocationDtoToStreamLocations(locations);
        }

        private IEnumerable<StreamLocation> MapLocationDtoToStreamLocations(List<GetLocationsResponseDto> locations)
        {
            return locations.Select(x => new StreamLocation { Location = x.Location }).ToList();
        }
    }
}