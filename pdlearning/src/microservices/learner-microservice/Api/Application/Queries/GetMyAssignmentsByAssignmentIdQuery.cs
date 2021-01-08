using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyAssignmentsByAssignmentIdQuery : BaseThunderQuery<MyAssignmentModel>
    {
        public Guid AssignmentId { get; set; }
    }
}
