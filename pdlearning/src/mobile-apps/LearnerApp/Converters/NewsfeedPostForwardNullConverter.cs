using System;
using System.Globalization;
using LearnerApp.Models.Newsfeed;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class NewsfeedPostForwardNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var forward = value as Forward;

            return forward != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
