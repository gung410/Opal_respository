using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CompletePostEvaluationCommand : BaseThunderCommand
    {
        public Guid RegistrationId { get; set; }
    }
}
