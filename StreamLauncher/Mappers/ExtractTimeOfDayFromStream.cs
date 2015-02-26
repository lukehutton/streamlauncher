using System;
using System.Globalization;
using System.Reflection;
using log4net;
using StreamLauncher.Models;

namespace StreamLauncher.Mappers
{
    public class ExtractTimeOfDayFromStream : IExtractTimeOfDayFromStream
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TimeSpan ExtractTimeOfDay(HockeyStream stream)
        {
            var startTimeString = stream.StartTime.ToLowerInvariant();
            var is24HourTime = !(startTimeString.Contains("pm") || startTimeString.Contains("am"));
            var timeWithoutTimeZone = stream.StartTime.Substring(0, stream.StartTime.LastIndexOf(' '));

            var @default = DateTime.Now;
            var startTime = new DateTime(@default.Year, @default.Month, @default.Day, 0, 0, 0); // default midnight
            try
            {
                var format = !is24HourTime ? "h:mm tt" : "HH:mm";
                startTime = DateTime.ParseExact(timeWithoutTimeZone, format, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Could not extract time of day from stream. Time string = {0}", stream.StartTime), ex);
            }
            return startTime.TimeOfDay; 
        }
    }
}