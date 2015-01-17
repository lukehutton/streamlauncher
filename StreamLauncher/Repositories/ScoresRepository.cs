using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using StreamLauncher.Api;
using StreamLauncher.Dtos;
using StreamLauncher.Models;

namespace StreamLauncher.Repositories
{
    public class ScoresRepository : IScoresRepository
    {
        private readonly IHockeyStreamsApiRequiringScoresApiKey _hockeyStreamsApi;

        public ScoresRepository(IHockeyStreamsApiRequiringScoresApiKey hockeyStreamsApi)
        {
            _hockeyStreamsApi = hockeyStreamsApi;            
        }

        public IEnumerable<Score> GetScores()
        {            
            var request = new RestRequest { Resource = "Scores" };
            request.AddParameter("date", DateTime.Now.ToString("MM/dd/yyyy"), ParameterType.GetOrPost);
            var scoresResponseDto = _hockeyStreamsApi.Execute<ScoresResponseDto>(request);
            return MapScoreDtoToScores(scoresResponseDto);        
        }

        private IEnumerable<Score> MapScoreDtoToScores(ScoresResponseDto scoreDtos)
        {
            return scoreDtos.Scores.Select(x => new Score
            {
                EventType = EventTypeParser.Parse(x.Event),
                HomeScore = Convert.ToInt32(x.HomeScore),
                HomeTeam = x.HomeTeam,
                AwayScore = Convert.ToInt32(x.AwayScore),
                AwayTeam = x.AwayTeam,
                PeriodAndTimeLeft = x.Period
            }).ToList();
        }
    }
}