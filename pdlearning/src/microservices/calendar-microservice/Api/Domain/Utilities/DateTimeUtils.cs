using System;

namespace Microservice.Calendar.Domain.Utilities
{
    public static class DateTimeUtils
    {
        public static DateTime ToStartOfDay(this DateTime dateTime)
        {
            return dateTime.RemoveTime();
        }

        public static DateTime ToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);
        }

        public static DateTime RemoveTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }

        public static DateTime ToFirstDayOfMonth(this DateTime dateTime)
        {
            return dateTime.AddDays(-dateTime.Day + 1);
        }

        public static DateTime ToLastDayOfMonth(this DateTime dateTime)
        {
            return dateTime.AddDays(DateTime.DaysInMonth(dateTime.Year, dateTime.Month) - dateTime.Day);
        }
    }
}
