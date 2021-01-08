using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteAssessmentAnswerCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
