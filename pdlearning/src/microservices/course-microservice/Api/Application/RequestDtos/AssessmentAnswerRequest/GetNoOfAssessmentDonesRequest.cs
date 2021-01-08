using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetNoOfAssessmentDonesRequest
    {
        public IEnumerable<Guid> ParticipantAssignmentTrackIds { get; set; }
    }
}
