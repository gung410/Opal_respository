using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class ApplyBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color applyColor = (Color)Application.Current.Resources["MainBodyTextColor"];

            if (value == null)
            {
                return applyColor;
            }

            if (!(value is bool))
            {
                return applyColor;
            }

            var canApply = (bool)value;

            return canApply ? applyColor : (Color)Application.Current.Resources["AppliedColor"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
