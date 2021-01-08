using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class MigrateLearningProcessCommand : BaseThunderCommand
    {
        public List<Guid> CourseIds { get; set; }

        public int BatchSize { get; set; }
    }
}
