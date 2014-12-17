using System.Collections.Generic;
using RestSharp;
using StreamLauncher.Dtos;
using StreamLauncher.Models;
using StreamLauncher.Providers;
using System.Linq;

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
            var request = new RestRequest {Resource = "GetLocations"};
            var locations = _hockeyStreamsApi.Execute<List<LocationDto>>(request);                        
            return MapLocationDtoToStreamLocations(locations);
        }

        private IEnumerable<StreamLocation> MapLocationDtoToStreamLocations(List<LocationDto> locations)
        {
            return locations.Select(x => new StreamLocation { Location = x.Location }).ToList();
        }
    }
}