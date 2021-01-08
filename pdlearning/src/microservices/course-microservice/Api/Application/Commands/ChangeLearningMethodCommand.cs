using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeLearningMethodCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public bool LearningMethod { get; set; }
    }
}
