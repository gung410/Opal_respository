using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters.Assignment
{
    public class AssignmentStatusBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color backgroundColor = (Color)Application.Current.Resources["DraftColor"];

            if (!(value is ParticipantAssignmentTrackStatus))
            {
                return backgroundColor;
            }

            var myAssignmentStatus = (ParticipantAssignmentTrackStatus)value;
            return myAssignmentStatus switch
            {
                ParticipantAssignmentTrackStatus.NotStarted => (Color)Application.Current.Resources["DraftColor"],
                ParticipantAssignmentTrackStatus.InProgress => (Color)Application.Current.Resources["InProgress"],
                ParticipantAssignmentTrackStatus.Completed => (Color)Application.Current.Resources["PublishedClassRunColor"],
                ParticipantAssignmentTrackStatus.LateSubmission => (Color)Application.Current.Resources["LateInSubmission"],
                ParticipantAssignmentTrackStatus.Incomplete => (Color)Application.Current.Resources["UnpublishedColor"],
                _ => backgroundColor
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
