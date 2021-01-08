using System;
using System.Globalization;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public enum LearningControlType
    {
        Card,
        Tile,
        LearningPath
    }

    public class ThumbnailUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                return stringValue;
            }

            string thumbnailUrl = string.Empty;
            if (parameter is string stringParameter && !string.IsNullOrEmpty(stringParameter))
            {
                switch (parameter)
                {
                    case nameof(LearningControlType.Card):
                        thumbnailUrl = "image_place_holder_h150.png";
                        break;
                    case nameof(LearningControlType.Tile):
                        thumbnailUrl = "image_place_holder_h60.png";
                        break;
                    case nameof(LearningControlType.LearningPath):
                        thumbnailUrl = "learning_path_thumbnail.png";
                        break;
                    default:
                        break;
                }
            }

            return thumbnailUrl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
