using System;
using System.Collections.Generic;
using System.Globalization;
using LearnerApp.Models.MyLearning;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class LearningPathsTotalItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = value as List<LearningPathsCourse>;

            if (items != null)
            {
                string label = items.Count > 1 ? "items" : "item";

                return $"{items.Count} {label}";
            }

            return "0 item";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
