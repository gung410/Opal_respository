using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class CourseLearningStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color backgroundColor = (Color)Application.Current.Resources["InProgress"];

            if (!(value is MyCourseStatus))
            {
                return backgroundColor;
            }

            var myCourseStatus = (MyCourseStatus)value;

            if (!string.IsNullOrEmpty(myCourseStatus.Status) && myCourseStatus.Status.Equals(StatusLearning.Completed.ToString()))
            {
                return (Color)Application.Current.Resources["Completed"];
            }

            if (!string.IsNullOrEmpty(myCourseStatus.Status) && myCourseStatus.Status.Equals(StatusLearning.Failed.ToString()))
            {
                return (Color)Application.Current.Resources["UnpublishedColor"];
            }

            if (!string.IsNullOrEmpty(myCourseStatus.Status) && myCourseStatus.Status.Equals(StatusLearning.Incomplete.ToString()))
            {
                return (Color)Application.Current.Resources["UnpublishedColor"];
            }

            return backgroundColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
