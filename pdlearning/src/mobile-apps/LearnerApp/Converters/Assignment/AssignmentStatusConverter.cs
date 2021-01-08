using System;
using System.Globalization;
using LearnerApp.Common;
using Xamarin.Forms;

namespace LearnerApp.Converters.Assignment
{
    public class AssignmentStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = string.Empty;

            if (!(value is ParticipantAssignmentTrackStatus))
            {
                return status;
            }

            var myAssignmentStatus = (ParticipantAssignmentTrackStatus)value;
            return myAssignmentStatus switch
            {
                ParticipantAssignmentTrackStatus.NotStarted => "Not-Started",
                ParticipantAssignmentTrackStatus.InProgress => "In-Progress",
                ParticipantAssignmentTrackStatus.Completed => ParticipantAssignmentTrackStatus.Completed.ToString(),
                ParticipantAssignmentTrackStatus.LateSubmission => "Late in Submission",
                ParticipantAssignmentTrackStatus.Incomplete => ParticipantAssignmentTrackStatus.Incomplete.ToString(),
                _ => status
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
