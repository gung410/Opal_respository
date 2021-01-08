using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public static class DigitalContentExtensionConverterHelper
    {
        public static string Convert(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return "document.svg";
            }

            var strValue = value.ToString().ToLower();
            return strValue == "picture-file" ? "picture_file.svg" : $"{strValue}.svg";
        }
    }

    /// <summary>
    /// This is for Digital Content only.
    /// </summary>
    public class FileExtensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DigitalContentExtensionConverterHelper.Convert(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
