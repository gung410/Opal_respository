using System;
using System.Globalization;
using LearnerApp.Models.StandaloneForm;
using Xamarin.Forms;

namespace LearnerApp.Converters.StandaloneForm
{
    public static class StandaloneFormTypeIconConverterHelper
    {
        public static string Convert(StandaloneFormTypeEnum? value)
        {
            switch (value)
            {
                case StandaloneFormTypeEnum.Poll:
                    return "standalone_form_poll.svg";
                case StandaloneFormTypeEnum.Survey:
                    return "standalone_form_survey.svg";
                case StandaloneFormTypeEnum.Quiz:
                    return "standalone_form_quiz.svg";
                default:
                    return "standalone_form_quiz.svg";
            }
        }
    }

    public class StandaloneFormTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return StandaloneFormTypeIconConverterHelper.Convert(value as StandaloneFormTypeEnum?);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
