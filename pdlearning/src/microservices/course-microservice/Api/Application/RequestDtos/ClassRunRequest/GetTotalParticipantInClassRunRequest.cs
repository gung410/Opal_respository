using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetTotalParticipantInClassRunRequest
    {
        public List<Guid> ClassRunIds { get; set; }
    }
}
