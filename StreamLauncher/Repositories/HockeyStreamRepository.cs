using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using StreamLauncher.Dtos;
using StreamLauncher.Models;
using StreamLauncher.Providers;

namespace StreamLauncher.Repositories
{
    public class HockeyStreamRepository : IHockeyStreamRepository
    {
        private readonly IHockeyStreamsApiRequiringToken _hockeyStreamsApi;

        public HockeyStreamRepository(IHockeyStreamsApiRequiringToken hockeyStreamsApi)
        {
            _hockeyStreamsApi = hockeyStreamsApi;
        }

        public Task<IEnumerable<HockeyStream>> GetLiveStreams(DateTime date)
        {
            var request = new RestRequest { Resource = "GetLive", Method = Method.GET };
            request.AddParameter("date", date.ToString("MM/dd/yyyy"), ParameterType.GetOrPost);
            var responseDto = _hockeyStreamsApi.Execute<GetLiveStreamsResponseDto>(request);
            var hockeyStreams = MapLiveStreamsResponseDtoToHockeyStreams(responseDto);
            return Task.FromResult(hockeyStreams);
        }

        // todo unit test and separate aggregation into another dependency and test?
        private IEnumerable<HockeyStream> MapLiveStreamsResponseDtoToHockeyStreams(GetLiveStreamsResponseDto responseDto)
        {
            // todo need scores api to see time left and period number
            return new List<HockeyStream>()
            {
                new HockeyStream()
            };
        }
    }
}