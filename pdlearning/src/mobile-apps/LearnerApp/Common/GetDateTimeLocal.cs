using System;
using System.Globalization;

namespace LearnerApp.Common
{
    public static class GetDateTimeLocal
    {
        public static DateTime Convert(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return default;
            }

            var dateTimeIso = dateTime.Value.ToString("o", CultureInfo.InvariantCulture);

            return DateTime.Parse(dateTimeIso);
        }
    }
}
