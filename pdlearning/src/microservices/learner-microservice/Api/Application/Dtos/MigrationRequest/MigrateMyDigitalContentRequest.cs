using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class MigrateMyDigitalContentRequest
    {
        public MigrationEventType MigrationEventType { get; set; }

        public List<Guid> OriginalObjectIds { get; set; }

        public int BatchSize { get; set; }

        public List<MyDigitalContentStatus> Statuses { get; set; }
    }
}
