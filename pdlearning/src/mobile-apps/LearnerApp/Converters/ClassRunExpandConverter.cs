using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class ClassRunExpandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "class_run_right_arrow.svg";
            }

            if (!(value is bool))
            {
                return "class_run_right_arrow.svg";
            }

            bool isExpand = (bool)value;

            return isExpand ? "class_run_down_arrow.svg" : "class_run_right_arrow.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
