using System;
using Microservice.Calendar.Application.Consumers.Messages.Models;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class AssignmentParticipantTrackChangedMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CourseAssignmentModel Assignment { get; set; }

        public CourseParticipantModel Participant { get; set; }
    }
}
