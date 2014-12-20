using System.Collections.Generic;
using StreamLauncher.Dtos;
using StreamLauncher.Models;

namespace StreamLauncher.Providers
{
    public interface ILiveStreamScheduleAggregatorAndMapper
    {
        IEnumerable<HockeyStream> AggregateAndMap(GetLiveStreamsResponseDto getLiveStreamsResponseDto);
    }
}