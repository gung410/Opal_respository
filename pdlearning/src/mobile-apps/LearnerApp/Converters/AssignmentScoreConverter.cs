using System;
using System.Globalization;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters
{
    public class AssignmentScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (!(value is QuizAnswer)))
            {
                return "0/0";
            }

            var quizAnswer = (QuizAnswer)value;
            var score = quizAnswer.Score;
            var total = (quizAnswer.ScorePercentage != 0) ? Math.Ceiling(score * 100 / quizAnswer.ScorePercentage) : 0;
            return $"{score}/{total}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
