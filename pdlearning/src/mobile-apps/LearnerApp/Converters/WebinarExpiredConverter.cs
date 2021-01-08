using System;
using System.Globalization;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class WebinarExpiredConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Session session)
            {
                if (session.LearningMethod && (session.GetEndDateTime() - DateTime.Now).TotalMinutes >= -30)
                {
                    return false;
                }
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}