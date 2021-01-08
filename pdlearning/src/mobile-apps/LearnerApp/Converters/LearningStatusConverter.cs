using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class LearningStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
            {
                return string.Empty;
            }

            var learningStatus = value.ToString();

            if (string.IsNullOrEmpty(learningStatus))
            {
                return string.Empty;
            }

            return learningStatus switch
            {
                nameof(CourseLearningStatus.ADDTOPDPLAN) => "ADD TO PD PLAN",
                nameof(CourseLearningStatus.START) => "START LEARNING",
                nameof(CourseLearningStatus.REMOVEFROMPDPLAN) => "REMOVE FROM PD PLAN",
                _ => learningStatus,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
