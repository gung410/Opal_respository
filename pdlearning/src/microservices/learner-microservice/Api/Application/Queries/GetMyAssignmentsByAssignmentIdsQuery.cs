using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyAssignmentsByAssignmentIdsQuery : BaseThunderQuery<List<MyAssignmentModel>>
    {
        public List<Guid> AssignmentIds { get; set; }
    }
}
