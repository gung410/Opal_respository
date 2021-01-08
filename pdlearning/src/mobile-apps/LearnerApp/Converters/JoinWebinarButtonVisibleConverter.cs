using System;
using System.Globalization;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class JoinWebinarButtonVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Session session)
            {
                var webinarExpiredConverter = (WebinarExpiredConverter)Application.Current.Resources["WebinarExpiredConverter"];
                if (webinarExpiredConverter != null)
                {
                    return !((bool)webinarExpiredConverter.Convert(session, null, null, null));
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}