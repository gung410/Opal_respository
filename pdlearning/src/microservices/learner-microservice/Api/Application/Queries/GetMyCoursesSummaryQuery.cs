using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyCoursesSummaryQuery : BaseThunderQuery<List<MyCoursesSummaryModel>>
    {
        public MyCourseStatus[] StatusFilter { get; set; }
    }
}
