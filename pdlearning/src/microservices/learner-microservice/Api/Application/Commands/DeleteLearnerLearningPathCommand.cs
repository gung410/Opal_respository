using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class DeleteLearnerLearningPathCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
