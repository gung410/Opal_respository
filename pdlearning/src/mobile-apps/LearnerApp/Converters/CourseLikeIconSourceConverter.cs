using System;
using System.Globalization;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class CourseLikeIconSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LearningOpportunityInformationCardTransfer info)
            {
                if (info.CourseExtendedInformation.IsLike)
                {
                    return "liked.svg";
                }
            }

            return "like.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
