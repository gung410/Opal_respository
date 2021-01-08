using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeRegistrationStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public RegistrationStatus Status { get; set; }

        public string Comment { get; set; }
    }
}
