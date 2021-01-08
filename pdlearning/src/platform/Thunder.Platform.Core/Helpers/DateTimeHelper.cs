using System;
using System.Runtime.InteropServices;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Thunder.Platform.Core.Helpers
{
    public class DateTimeHelper
    {
        /// <summary>
        /// To get a day before X days except weekend (Saturday and Sunday).
        /// </summary>
        /// <param name="numberWorkingDayInput">The number of days to compare.</param>
        /// <returns>The day before X days.</returns>
        public static DateTime GetDayBeforeXWorkingDayFromNow(int numberWorkingDayInput)
        {
            int numberWorkingDay = 0;

            var today = Clock.Now;
            do
            {
                today = today.AddDays(-1);
                if (IsWeekend(today))
                {
                    continue;
                }

                numberWorkingDay++;
            }
            while (numberWorkingDay != numberWorkingDayInput);

            return today;
        }

        public static bool IsWeekend(DateTime day)
        {
            return day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// To get system timezone.
        /// </summary>
        /// <param name="windowTimezoneId">The ID of window timezone.</param>
        /// <param name="linuxTimezoneId">The ID of linux timezone.</param>
        /// <returns>The System Timezone.</returns>
        public static TimeZoneInfo GetSystemTimeZone(string windowTimezoneId, string linuxTimezoneId)
        {
            TimeZoneInfo systemTimeZone = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
               ? TimeZoneInfo.FindSystemTimeZoneById(linuxTimezoneId)
               : TimeZoneInfo.FindSystemTimeZoneById(windowTimezoneId);

            return systemTimeZone;
        }

        public static DateTime EndOfTodayInSystemTimeZone()
        {
            return Clock.Now.EndOfDateInSystemTimeZone();
        }

        public static DateTime StartOfTodayInSystemTimeZone()
        {
            return Clock.Now.StartOfDateInSystemTimeZone();
        }
    }
}
