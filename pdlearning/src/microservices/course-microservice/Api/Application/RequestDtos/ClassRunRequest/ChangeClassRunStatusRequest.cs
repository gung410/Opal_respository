using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeClassRunStatusRequest
    {
        public List<Guid> Ids { get; set; }

        public ClassRunStatus Status { get; set; }
    }
}
