using System;
using System.Runtime.InteropServices;
using Microservice.Calendar.Infrastructure.Settings;

namespace Microservice.Calendar.Infrastructure.Helpers
{
    public static class TimeHelper
    {
        public static TimeZoneInfo GetSystemTimeZoneInfo(NotificationTimeZoneOption systemTimeZoneOption)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return TimeZoneInfo.FindSystemTimeZoneById(systemTimeZoneOption.Linux);
            }

            return TimeZoneInfo.FindSystemTimeZoneById(systemTimeZoneOption.Windows);
        }
    }
}
