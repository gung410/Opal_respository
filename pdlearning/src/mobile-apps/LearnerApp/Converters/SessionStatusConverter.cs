using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class SessionStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color publishedColor = (Color)Application.Current.Resources["PublishedClassRunColor"];

            if (value == null)
            {
                return publishedColor;
            }

            var sessionStatus = value.ToString();

            return sessionStatus switch
            {
                nameof(SessionStatus.Completed) => (Color)Application.Current.Resources["PublishedClassRunColor"],
                nameof(SessionStatus.Incomplete) => (Color)Application.Current.Resources["CancelledColor"],
                _ => publishedColor,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
