using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class LearningStatusVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MyCourseStatus myCourseStatus)
            {
                var statusEnableConverter = (LearningStatusEnableConverter)Application.Current.Resources["LearningStatusEnableConverter"];
                bool isEnable = (bool)statusEnableConverter.Convert(myCourseStatus, null, null, null);
                if (myCourseStatus.LearningStatus == nameof(CourseLearningStatus.ADDTOPDPLAN) || myCourseStatus.LearningStatus == nameof(CourseLearningStatus.REMOVEFROMPDPLAN))
                {
                    if ((myCourseStatus.IsVisibleLearningStatus && !isEnable) || myCourseStatus.IsCourseCompleted)
                    {
                        return false;
                    }
                }

                return myCourseStatus.IsVisibleLearningStatus;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
