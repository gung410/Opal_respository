using System;
using System.Collections.Generic;

namespace Microservice.Form.Application.RequestDtos
{
    public class RemindFormParticipantRequest
    {
        public List<Guid> ParticipantIds { get; set; }

        public Guid FormId { get; set; }
    }
}
