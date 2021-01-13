using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class ClassRunTime12HourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is DateTime?))
            {
                return null;
            }

            var dateTime = (DateTime?)value;

            var dateTimeISO = dateTime.Value.ToString("o", CultureInfo.InvariantCulture);

            var classRunDateTime = DateTime.Parse(dateTimeISO);

            return $"{classRunDateTime:hh:mm tt}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}