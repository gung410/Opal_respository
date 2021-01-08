using System;
using System.Globalization;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class JoinWebinarButtonEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Session session)
            {
                if (session.LearningMethod && (DateTime.Now - session.GetStartDateTime()).TotalMinutes >= -30 && (session.GetEndDateTime() - DateTime.Now).TotalMinutes >= -30)
                {
                    return true;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}