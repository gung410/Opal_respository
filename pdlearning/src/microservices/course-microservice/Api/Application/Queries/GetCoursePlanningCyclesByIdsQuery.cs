using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetCoursePlanningCyclesByIdsQuery : BaseThunderQuery<List<CoursePlanningCycleModel>>
    {
        public List<Guid> Ids { get; set; }
    }
}
