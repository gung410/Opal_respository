using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class ConfirmButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color availableColor = (Color)Application.Current.Resources["DefaultTextColor"];
            Color unavailableColor = Color.LightGray;
            if (value == null || !(value is bool))
            {
                return unavailableColor;
            }

            return (bool)value ? availableColor : unavailableColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
