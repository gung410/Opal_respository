using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeClassRunCommand : BaseThunderCommand
    {
        public List<Guid> RegistrationIds { get; set; }

        public Guid ClassRunChangeId { get; set; }
    }
}
