using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyOutstandingTaskByIdQuery : BaseThunderQuery<OutstandingTaskModel>
    {
        public Guid Id { get; set; }
    }
}
