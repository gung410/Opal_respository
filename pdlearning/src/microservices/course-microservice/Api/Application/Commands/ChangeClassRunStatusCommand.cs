using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeClassRunStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public ClassRunStatus Status { get; set; }
    }
}
