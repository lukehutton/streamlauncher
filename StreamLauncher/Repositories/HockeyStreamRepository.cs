using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
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
    public class HockeyStreamRepository : IHockeyStreamRepository
    {
        private readonly ILiveStreamScheduleAggregatorAndMapper _aggregatorAndMapper;
        private readonly IHockeyStreamsApiRequiringToken _hockeyStreamsApi;
        private readonly IScoresRepository _scoresRepository;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            var dateParameter = new Parameter
            {
                Name = "date",
                Value = date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                Type = ParameterType.GetOrPost
            };
            request.AddParameter(dateParameter);
            
            Log.InfoFormat("Retrieving live streams with parameter: {0}-{1}", dateParameter.Name, dateParameter.Value);
            var responseDto = _hockeyStreamsApi.Execute<GetLiveStreamsResponseDto>(request);
            Log.InfoFormat("Retrieved {0} live streams", responseDto.Schedule.Count);

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
            var idParameter = new Parameter
            {
                Name = "id",
                Value = streamId,
                Type = ParameterType.GetOrPost
            };
            var locationParameter = new Parameter
            {
                Name = "location",
                Value = location,
                Type = ParameterType.GetOrPost
            };
            request.AddParameter(idParameter);
            request.AddParameter(locationParameter);
            
            Log.InfoFormat("Retrieving live stream with parameters: {0}-{1}, {2}-{3}",
                idParameter.Name, idParameter.Value, locationParameter.Name, locationParameter.Value
                );
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
                    Log.InfoFormat("Retrieved SD live stream with source: {0}", liveStream.Source);
                    break;
                case Quality.HD:
                    if (responseDto.TrueLiveHD.Count == 0)
                    {
                        StreamNotFoundException(streamId, location, quality);
                    }
                    liveStream.Source = responseDto.TrueLiveHD.First().Src;
                    Log.InfoFormat("Retrieved HD live stream with source: {0}", liveStream.Source);
                    break;
            }
            return Task.FromResult(liveStream);
        }

        private void StreamNotFoundException(int streamId, string location, Quality quality)
        {            
            var exception = new StreamNotFoundException(
                string.Format("StreamId: {0}, Location: {1}, Quality: {2} Not Found", streamId, location,
                    quality));
            Log.Error(exception.Message, exception);
            throw exception;
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