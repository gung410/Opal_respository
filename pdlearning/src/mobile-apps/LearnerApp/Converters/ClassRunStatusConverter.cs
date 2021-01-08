using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class ClassRunStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color publishedColor = (Color)Application.Current.Resources["PublishedClassRunColor"];

            if (value == null)
            {
                return publishedColor;
            }

            var classRunStatus = value.ToString();

            return classRunStatus switch
            {
                nameof(ClassRunStatus.Cancelled) => (Color)Application.Current.Resources["CancelledColor"],
                nameof(ClassRunStatus.Unpublished) => (Color)Application.Current.Resources["UnpublishedColor"],
                _ => publishedColor,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
