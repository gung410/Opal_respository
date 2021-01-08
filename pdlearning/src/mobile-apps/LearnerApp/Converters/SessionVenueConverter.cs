using System;
using System.Globalization;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class SessionVenueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Session session)
            {
                if (session.LearningMethod)
                {
                    return "Online Session";
                }
                else
                {
                    return session.Venue;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}