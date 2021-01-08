using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetLearnerLearningPathByIdsQuery : BaseThunderQuery<List<LearnerLearningPathModel>>
    {
        public Guid[] Ids { get; set; }
    }
}
