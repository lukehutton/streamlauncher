using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using StreamLauncher.Api;
using StreamLauncher.Dtos;
using StreamLauncher.Mappers;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public class HockeyStreamRepository : IHockeyStreamRepository
    {
        private readonly IHockeyStreamsApiRequiringToken _hockeyStreamsApi;
        private readonly ILiveStreamScheduleAggregatorAndMapper _aggregatorAndMapper;

        public HockeyStreamRepository(
            IHockeyStreamsApiRequiringToken hockeyStreamsApi,
            ILiveStreamScheduleAggregatorAndMapper aggregatorAndMapper
            )
        {
            _hockeyStreamsApi = hockeyStreamsApi;
            _aggregatorAndMapper = aggregatorAndMapper;
        }

        public Task<IEnumerable<HockeyStream>> GetLiveStreams(DateTime date)
        {
            var request = new RestRequest { Resource = "GetLive", Method = Method.GET };
            request.AddParameter("date", date.ToString("MM/dd/yyyy"), ParameterType.GetOrPost);
            var responseDto = _hockeyStreamsApi.Execute<GetLiveStreamsResponseDto>(request);
            var hockeyStreams = _aggregatorAndMapper.AggregateAndMap(responseDto);
            return Task.FromResult(hockeyStreams);
        }        
    }
}