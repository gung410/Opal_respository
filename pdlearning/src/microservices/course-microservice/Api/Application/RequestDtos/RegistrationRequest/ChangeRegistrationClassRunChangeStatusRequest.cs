using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeRegistrationClassRunChangeStatusRequest
    {
        public List<Guid> Ids { get; set; }

        public ClassRunChangeStatus ClassRunChangeStatus { get; set; }

        public string Comment { get; set; }
    }
}
