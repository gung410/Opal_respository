using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class SetupPeerAssessmentRequest
    {
        public Guid AssignmentId { get; set; }

        public Guid ClassrunId { get; set; }

        public int NumberAutoAssessor { get; set; }
    }
}
