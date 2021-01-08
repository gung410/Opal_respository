using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class LabelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Color.Transparent;
            }

            string status = value.ToString();

            if (string.IsNullOrEmpty(status))
            {
                return Color.Transparent;
            }

            var colorStatus = status switch
            {
                nameof(StatusLearning.NotStarted) => Color.LightGray,
                nameof(StatusLearning.Completed) => (Color)Application.Current.Resources["Completed"],
                nameof(StatusLearning.Expired) => (Color)Application.Current.Resources["ExpiredColor"],
                nameof(StatusCourse.Unpublished) => (Color)Application.Current.Resources["UnpublishedColor"],
                nameof(StatusLearning.Failed) => (Color)Application.Current.Resources["UnpublishedColor"],
                nameof(StatusLearning.Incomplete) => (Color)Application.Current.Resources["UnpublishedColor"],
                nameof(StatusLearning.Archived) => Color.LightGray,
                _ => (Color)Application.Current.Resources["InProgress"],
            };
            return colorStatus;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
