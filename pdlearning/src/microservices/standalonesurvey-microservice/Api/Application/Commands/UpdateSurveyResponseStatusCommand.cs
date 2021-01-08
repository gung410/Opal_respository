using System;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class UpdateSurveyResponseStatusCommand : BaseStandaloneSurveyCommand
    {
        public UpdateSurveyResponseStatusCommand(Guid formId, Guid userId, DateTime? attendanceTime, DateTime? submittedTime)
        {
            FormId = formId;
            UserId = userId;
            AttendanceTime = attendanceTime;
            SubmittedTime = submittedTime;
        }

        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public DateTime? AttendanceTime { get; set; }

        public DateTime? SubmittedTime { get; set; }
    }
}
