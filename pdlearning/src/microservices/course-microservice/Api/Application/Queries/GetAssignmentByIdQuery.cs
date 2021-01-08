using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAssignmentByIdQuery : BaseThunderQuery<AssignmentModel>
    {
        public Guid Id { get; set; }

        public bool ForLearnerAnswer { get; set; }
    }
}
