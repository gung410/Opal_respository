using System;
using System.Globalization;

namespace cxOrganization.Domain.Common
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Validate if the input date match the format
        /// </summary>
        /// <param name="date">the input date to check.</param>
        /// <param name="format">the date format to check (e.g. MM/dd/yyyy).</param>
        /// <returns>boolean</returns>
        public static bool isMatchFormat(string date, string format)
        {
            return DateTime.TryParseExact(
                    date,
                    format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, 
                    out  var dateTime);
        }

        /// <summary>
        /// Get the week number of month based on date 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeekOfMonth(DateTime date)
        {
            return (date.Day + 6) / 7;
        }
    }
}
