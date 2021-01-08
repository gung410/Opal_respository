using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class MigratePdRecordRequestDto
    {
        public MigrationEventType MigrationEventType { get; set; }

        public List<Guid> CourseIds { get; set; }

        public int BatchSize { get; set; }

        public List<MyCourseStatus> Statuses { get; set; }
    }
}
