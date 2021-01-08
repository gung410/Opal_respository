using System;

namespace Microservice.Course.Application.Models
{
    public class NoOfAssessmentDoneInfoModel
    {
        public Guid ParticipantAssignmentTrackId { get; set; }

        public int TotalAssessments { get; set; }

        public int DoneAssessments { get; set; }
    }
}
