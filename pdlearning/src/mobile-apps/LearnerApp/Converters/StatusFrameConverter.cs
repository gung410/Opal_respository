using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class StatusFrameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (!(value is MyCourseStatus))
            {
                return false;
            }

            var myCourseStatus = (MyCourseStatus)value;

            if (string.IsNullOrEmpty(myCourseStatus.Status)
                && string.IsNullOrEmpty(myCourseStatus.MyRegistrationStatus)
                && string.IsNullOrEmpty(myCourseStatus.MyWithdrawalStatus))
            {
                return false;
            }

            if (myCourseStatus.Status.Equals(StatusLearning.NotStarted.ToString())
                && string.IsNullOrEmpty(myCourseStatus.MyRegistrationStatus)
                && string.IsNullOrEmpty(myCourseStatus.MyWithdrawalStatus))
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
