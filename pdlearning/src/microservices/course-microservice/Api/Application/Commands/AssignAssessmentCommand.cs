using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class AssignAssessmentCommand : BaseThunderCommand
    {
        public Guid AssignmentId { get; set; }

        public List<Guid> ParticipantAssignmentTrackIds { get; set; }
    }
}
