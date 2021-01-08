using System;
using System.Runtime.InteropServices;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Infrastructure
{
    public static class TimeHelper
    {
        public static TimeZoneInfo GetSystemTimeZoneInfo(SystemTimeZoneOption systemTimeZoneOption)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return TimeZoneInfo.FindSystemTimeZoneById(systemTimeZoneOption.Linux);
            }

            return TimeZoneInfo.FindSystemTimeZoneById(systemTimeZoneOption.Windows);
        }

        public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo departureTimeZone, TimeZoneInfo destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, departureTimeZone, destinationTimeZone);
        }

        public static DateTime ConvertTimeFromUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, Clock.TimeZoneInfo);
        }
    }
}
