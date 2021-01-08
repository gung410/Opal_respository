using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class MassChangeClassRunRequest
    {
        public List<Guid> RegistrationIds { get; set; }

        public Guid ClassRunChangeId { get; set; }
    }
}
