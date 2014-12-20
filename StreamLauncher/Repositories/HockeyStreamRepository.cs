using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IScoresRepository _scoresRepository;

        public HockeyStreamRepository(
            IHockeyStreamsApiRequiringToken hockeyStreamsApi,
            ILiveStreamScheduleAggregatorAndMapper aggregatorAndMapper,
            IScoresRepository scoresRepository
            )
        {
            _hockeyStreamsApi = hockeyStreamsApi;
            _aggregatorAndMapper = aggregatorAndMapper;
            _scoresRepository = scoresRepository;
        }

        public Task<IEnumerable<HockeyStream>> GetLiveStreams(DateTime date)
        {
            var request = new RestRequest { Resource = "GetLive", Method = Method.GET };
            request.AddParameter("date", date.ToString("MM/dd/yyyy"), ParameterType.GetOrPost);
            var responseDto = _hockeyStreamsApi.Execute<GetLiveStreamsResponseDto>(request);
            var hockeyStreams = _aggregatorAndMapper.AggregateAndMap(responseDto);
            var scores = _scoresRepository.GetScores();
            var streamsWithScores = hockeyStreams.Select(stream =>
            {
                stream.PeriodAndTimeLeft = scores
                    .Where(score => score.HomeTeam == stream.HomeTeam && stream.IsPlaying)
                    .Select(p => p.PeriodAndTimeLeft)
                    .FirstOrDefault();
                return stream;
            });
            return Task.FromResult(streamsWithScores);
        }        
    }
}