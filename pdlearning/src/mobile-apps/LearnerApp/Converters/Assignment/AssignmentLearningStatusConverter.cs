using System;
using System.Globalization;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using Xamarin.Forms;

namespace LearnerApp.Converters.Assignment
{
    public class AssignmentLearningStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is AssignmentDetail))
            {
                return string.Empty;
            }

            var assignmentDetail = (AssignmentDetail)value;
            var dueDate = assignmentDetail.EndDate.AddDays(30).Date;
            var myAssignmentStatus = assignmentDetail.Status;
            return DateTime.Compare(DateTime.Now.ToUniversalTime().Date, dueDate) > 0 &&
                (myAssignmentStatus == ParticipantAssignmentTrackStatus.NotStarted ||
                    myAssignmentStatus == ParticipantAssignmentTrackStatus.InProgress ||
                    myAssignmentStatus == ParticipantAssignmentTrackStatus.Completed)
                ? "VIEW AGAIN"
                : myAssignmentStatus switch
                {
                    ParticipantAssignmentTrackStatus.NotStarted => "START",
                    ParticipantAssignmentTrackStatus.InProgress => "CONTINUE",
                    ParticipantAssignmentTrackStatus.Completed => "VIEW AGAIN",
                    ParticipantAssignmentTrackStatus.Incomplete => "VIEW AGAIN",
                    ParticipantAssignmentTrackStatus.LateSubmission => "VIEW AGAIN",
                    _ => "START"
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}