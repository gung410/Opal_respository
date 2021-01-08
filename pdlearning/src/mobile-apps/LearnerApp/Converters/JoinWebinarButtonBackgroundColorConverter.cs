using System;
using System.Globalization;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class JoinWebinarButtonBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color disabledColor = (Color)Application.Current.Resources["RegisteredColor"];
            Color enabledColor = (Color)Application.Current.Resources["MainBodyTextColor"];
            if (value is Session session)
            {
                var joinWebinarButtonEnableConverter = (JoinWebinarButtonEnableConverter)Application.Current.Resources["JoinWebinarButtonEnableConverter"];
                if (joinWebinarButtonEnableConverter != null)
                {
                    var enable = (bool)joinWebinarButtonEnableConverter.Convert(session, null, null, null);
                    return enable ? enabledColor : disabledColor;
                }
            }

            return disabledColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}