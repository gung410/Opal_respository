using System;
using System.Globalization;
using LearnerApp.Models.MyLearning;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class DigitalContentLikeIconSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MyLearningDigitalContentMetadataTransfer info)
            {
                if (info.IsLike)
                {
                    return "liked.svg";
                }
            }

            return "like.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
