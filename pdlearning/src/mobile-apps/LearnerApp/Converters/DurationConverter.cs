using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var durationCourse = string.Empty;

            if (value != null)
            {
                int duration = System.Convert.ToInt32(value.ToString());
                var hours = duration / 60;
                var minutes = duration % 60;

                var hoursCourse = hours > 0 ? $"{hours.ToString()}H" : string.Empty;
                var minutesCourse = minutes > 0 ? $"{minutes.ToString()}M" : string.Empty;

                durationCourse = $"{hoursCourse} {minutesCourse}";
            }

            return durationCourse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
