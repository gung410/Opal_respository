using System;
using System.Globalization;
using LearnerApp.Resources.Texts;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class CollectionViewEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (!(value is bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return (bool)value ? TextsResource.NOTHING_HERE_YET : "Loading...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
