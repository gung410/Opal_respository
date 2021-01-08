using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetSingleParticipantAssignmentTrackQuery : BaseThunderQuery<ParticipantAssignmentTrackModel>
    {
        public Guid RegistrationId { get; set; }

        public Guid AssignmentId { get; set; }
    }
}
