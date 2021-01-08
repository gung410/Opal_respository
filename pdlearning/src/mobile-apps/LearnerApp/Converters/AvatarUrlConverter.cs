using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class AvatarUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string url = (string)value;

            return string.IsNullOrEmpty(url) ? "default_avatar.png" : url;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
