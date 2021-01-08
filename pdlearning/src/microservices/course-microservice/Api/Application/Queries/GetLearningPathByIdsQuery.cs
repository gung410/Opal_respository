using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetLearningPathByIdsQuery : BaseThunderQuery<List<LearningPathModel>>
    {
        public Guid[] Ids { get; set; }
    }
}
