using System;
using System.Globalization;
using LearnerApp.Models;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class BookmarkInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bookmarkInfo = value as BookmarkInfo;

            return bookmarkInfo != null ? "bookmarked.svg" : "bookmark.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
