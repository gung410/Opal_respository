using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetLecturesInMyCourseQuery : BaseThunderQuery<List<LectureInMyCourseModel>>
    {
        public Guid MyCourseId { get; set; }
    }
}
