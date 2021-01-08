using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyCourseToMigrateMicroLearningQuery : BaseThunderQuery<int>
    {
        public MigrationEventType MigrationEventType { get; set; }

        public List<Guid> CourseIds { get; set; }

        public int BatchSize { get; set; }

        public List<MyCourseStatus> Statuses { get; set; }
    }
}
