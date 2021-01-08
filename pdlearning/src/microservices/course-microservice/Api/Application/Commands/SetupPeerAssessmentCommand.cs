using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SetupPeerAssessmentCommand : BaseThunderCommand
    {
        public Guid AssignmentId { get; set; }

        public Guid ClassrunId { get; set; }

        public int NumberAutoAssessor { get; set; }
    }
}
