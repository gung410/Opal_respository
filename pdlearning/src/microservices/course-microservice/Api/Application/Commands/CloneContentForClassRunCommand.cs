using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CloneContentForClassRunCommand : BaseThunderCommand
    {
        public Guid ClassRunId { get; set; }
    }
}
