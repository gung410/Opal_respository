using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class ClassRunInformationExpandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "arrow.svg";
            }

            if (!(value is bool))
            {
                return "arrow.svg";
            }

            bool isExpand = (bool)value;

            return isExpand ? "arrow_down.svg" : "arrow.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
