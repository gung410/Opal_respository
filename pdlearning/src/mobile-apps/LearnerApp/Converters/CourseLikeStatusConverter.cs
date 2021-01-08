using System;
using System.Globalization;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class CourseLikeStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LearningOpportunityInformationCardTransfer info)
            {
                var likeCount = info.CourseExtendedInformation.TotalLike;
                if (likeCount > 0)
                {
                    return likeCount == 1 ? "1 like" : $"{likeCount} likes";
                }
            }

            return "0 like";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
