using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteAssignmentCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }
    }
}
