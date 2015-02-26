using System;
using StreamLauncher.Models;

namespace StreamLauncher.Mappers
{
    public interface IExtractTimeOfDayFromStream
    {
        TimeSpan ExtractTimeOfDay(HockeyStream stream);
    }
}