using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeRegistrationStatusRequest
    {
        public string Comment { get; set; }

        public List<Guid> Ids { get; set; }

        public RegistrationStatus Status { get; set; }
    }
}
