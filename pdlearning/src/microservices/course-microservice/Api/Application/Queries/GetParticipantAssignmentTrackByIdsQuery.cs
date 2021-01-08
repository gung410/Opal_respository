using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetParticipantAssignmentTrackByIdsQuery : BaseThunderQuery<List<ParticipantAssignmentTrackModel>>
    {
        public List<Guid> Ids { get; set; }
    }
}
