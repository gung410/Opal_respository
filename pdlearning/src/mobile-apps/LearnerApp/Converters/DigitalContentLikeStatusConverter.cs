using System;
using System.Globalization;
using LearnerApp.Models.MyLearning;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class DigitalContentLikeStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MyLearningDigitalContentMetadataTransfer info)
            {
                var likeCount = info.TotalLike;
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
