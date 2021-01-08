using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyDigitalContentToMigrateQuery : BaseThunderQuery<int>
    {
        public MigrationEventType MigrationEventType { get; set; }

        public List<Guid> OriginalObjectIds { get; set; }

        public int BatchSize { get; set; }

        public List<MyDigitalContentStatus> Statuses { get; set; }
    }
}
