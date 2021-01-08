using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeRegistrationClassRunChangeStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public ClassRunChangeStatus ClassRunChangeStatus { get; set; }
    }
}
