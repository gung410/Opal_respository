using System;
using System.Globalization;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class CheckCardTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cardType = (BookmarkType)value;

            if (parameter == null)
            {
                return false;
            }

            return cardType.ToString().Equals(parameter as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
