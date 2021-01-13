using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class ClassRunSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (!(value is int))
            {
                return string.Empty;
            }

            var maxClassSize = (int)value;

            return maxClassSize > 1 ? $"{maxClassSize} persons" : $"{maxClassSize} person";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}