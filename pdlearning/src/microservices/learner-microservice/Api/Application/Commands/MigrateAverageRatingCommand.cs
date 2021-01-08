using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class MigrateAverageRatingCommand : BaseThunderCommand
    {
        public List<Guid> DigitalContentIds { get; set; }

        public int BatchSize { get; set; }
    }
}
