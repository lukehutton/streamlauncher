using System.Collections.Generic;
using StreamLauncher.Dtos;
using StreamLauncher.Models;

namespace StreamLauncher.Mappers
{
    public interface ILiveStreamScheduleAggregatorAndMapper
    {
        IEnumerable<HockeyStream> AggregateAndMap(GetLiveStreamsResponseDto getLiveStreamsResponseDto);
    }
}