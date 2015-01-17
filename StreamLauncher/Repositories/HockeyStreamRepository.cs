using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using StreamLauncher.Api;
using StreamLauncher.Constants;
using StreamLauncher.Dtos;
using StreamLauncher.Exceptions;
using StreamLauncher.Mappers;
using StreamLauncher.Models;
using StreamLauncher.Util;

namespace StreamLauncher.Repositories
{
    // todo unit test
    public class HockeyStreamRepository : IHockeyStreamRepository
    {
        private readonly ILiveStreamScheduleAggregatorAndMapper _aggregatorAndMapper;
        private readonly IHockeyStreamsApiRequiringToken _hockeyStreamsApi;
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
            var request = new RestRequest {Resource = "GetLive", Method = Method.GET};
            request.AddParameter("date", date.ToString("MM/dd/yyyy"), ParameterType.GetOrPost);
            var responseDto = _hockeyStreamsApi.Execute<GetLiveStreamsResponseDto>(request);
            var hockeyStreams = _aggregatorAndMapper.AggregateAndMap(responseDto);
            var scores = _scoresRepository.GetScores();
            var streamsWithScores = hockeyStreams.Select(stream =>
            {
                stream.PeriodAndTimeLeft = scores
                    .Where(
                        score =>
                            score.HomeTeam.MaxStrLen(AppConstants.MaxTeamStringLength) == stream.HomeTeam &&
                            stream.IsPlaying)
                    .Select(p => p.PeriodAndTimeLeft)
                    .FirstOrDefault() ?? "-";
                DeterminePeriodAndScore(stream);

                return stream;
            });
            return Task.FromResult(streamsWithScores);
        }

        public Task<LiveStream> GetLiveStream(int streamId, string location, Quality quality)
        {
            var request = new RestRequest {Resource = "GetLiveStream", Method = Method.GET};
            request.AddParameter("id", streamId, ParameterType.GetOrPost);
            request.AddParameter("location", location, ParameterType.GetOrPost);
            var responseDto = _hockeyStreamsApi.Execute<GetLiveStreamResponseDto>(request);
            var liveStream = new LiveStream();
            switch (quality)
            {
                case Quality.SD:
                    if (responseDto.TrueLiveSD.Count == 0)
                    {
                        StreamNotFoundException(streamId, location, quality);
                    }
                    liveStream.Source = responseDto.TrueLiveSD.First().Src;
                    break;
                case Quality.HD:
                    if (responseDto.TrueLiveHD.Count == 0)
                    {
                        StreamNotFoundException(streamId, location, quality);
                    }
                    liveStream.Source = responseDto.TrueLiveHD.First().Src;
                    break;
            }
            return Task.FromResult(liveStream);
        }

        private static void StreamNotFoundException(int streamId, string location, Quality quality)
        {
            throw new StreamNotFoundException(
                string.Format("StreamId: {0}, Location: {1}, Quality: {2} Not Found", streamId, location,
                    quality));
        }

        private static void DeterminePeriodAndScore(HockeyStream stream)
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
                if (stream.StartTimeSpan < DateTime.Now.TimeOfDay)
                {
                    stream.PeriodAndTimeLeft = "Final";
                }
            }
            if (stream.PeriodAndTimeLeft == "-")
            {
                stream.Score = "n/a";
            }
        }
    }

    public enum Quality
    {
        SD,
        HD
    }
}