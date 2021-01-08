using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class SessionFrameVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var sessionStatus = value.ToString();

            return sessionStatus switch
            {
                nameof(SessionStatus.Completed) => true,
                nameof(SessionStatus.Incomplete) => true,
                _ => false,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
