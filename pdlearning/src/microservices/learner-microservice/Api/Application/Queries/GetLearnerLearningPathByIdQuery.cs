using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetLearnerLearningPathByIdQuery : BaseThunderQuery<LearnerLearningPathModel>
    {
        public Guid Id { get; set; }
    }
}
