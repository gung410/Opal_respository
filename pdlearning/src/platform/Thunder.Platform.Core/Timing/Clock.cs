using System;

namespace Thunder.Platform.Core.Timing
{
    public static class Clock
    {
        static Clock()
        {
            Provider = new DotNetClockProvider();
            TimeZoneInfo = TimeZoneInfo.Local;
        }

        public static IClockProvider Provider { get; private set; }

        public static DateTime Now => Provider.Now;

        public static DateTimeKind Kind => Provider.Kind;

        /// <summary>
        /// Timezone info of the clock.
        /// </summary>
        public static TimeZoneInfo TimeZoneInfo { get; private set; }

        public static DateTime Normalize(DateTime dateTime)
        {
            return Provider.Normalize(dateTime);
        }

        public static void SetProvider(IClockProvider clockProvider)
        {
            Provider = clockProvider ?? throw new ArgumentNullException(nameof(clockProvider));
        }

        public static void SetTimeZoneInfo(TimeZoneInfo timeZoneInfo)
        {
            TimeZoneInfo = timeZoneInfo ?? throw new ArgumentNullException(nameof(timeZoneInfo));
        }
    }
}
