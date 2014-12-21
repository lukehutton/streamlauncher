using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using StreamLauncher.Api;
using StreamLauncher.Dtos;
using StreamLauncher.Mappers;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    // todo unit test
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
                    .FirstOrDefault() ?? "-";
                DeterminePeriod(stream);

                return stream;
            });
            return Task.FromResult(streamsWithScores);
        }

        private static void DeterminePeriod(HockeyStream stream)
        {
            try
            {
                if (!(stream.PeriodAndTimeLeft.Contains("1st") ||
                      stream.PeriodAndTimeLeft.Contains("2nd") ||
                      stream.PeriodAndTimeLeft.Contains("3rd") ||
                      stream.PeriodAndTimeLeft.Contains("OT") ||
                      stream.PeriodAndTimeLeft.Contains("SO")))
                {
                    stream.PeriodAndTimeLeft = "-";
                }
                if (!stream.IsPlaying)
                {
                    var timeWithoutTimeZone = stream.StartTime.Substring(0, stream.StartTime.LastIndexOf(' '));
                    var startTime = DateTime.ParseExact(timeWithoutTimeZone, "h:mm tt", CultureInfo.InvariantCulture);
                    if (startTime.TimeOfDay < DateTime.Now.TimeOfDay)
                    {
                        stream.PeriodAndTimeLeft = "Final";
                    }
                }
            }
            catch 
            {
                stream.PeriodAndTimeLeft = "-";
            }
        }
    }
}