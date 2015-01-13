using System.Collections.Generic;
using RestSharp;
using StreamLauncher.Dtos;
using StreamLauncher.Models;
using System.Linq;
using System.Runtime.Caching;
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
            ObjectCache cache = MemoryCache.Default;
            var streamLocations = cache["streamLocations"] as IEnumerable<StreamLocation>;

            if (streamLocations == null)
            {            
                var request = new RestRequest { Resource = "GetLocations" };
                var locations = _hockeyStreamsApi.Execute<List<GetLocationsResponseDto>>(request);
                streamLocations = MapLocationDtoToStreamLocations(locations);
                cache.Set("streamLocations", streamLocations, new CacheItemPolicy());
            }

            return streamLocations;
        }

        private IEnumerable<StreamLocation> MapLocationDtoToStreamLocations(List<GetLocationsResponseDto> locations)
        {
            return locations.Select(x => new StreamLocation { Location = x.Location }).ToList();
        }
    }
}