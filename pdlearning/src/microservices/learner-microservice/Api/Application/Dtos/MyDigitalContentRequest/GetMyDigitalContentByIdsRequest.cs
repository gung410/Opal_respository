using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class GetMyDigitalContentByIdsRequest
    {
        public IEnumerable<Guid> DigitalContentIds { get; set; }
    }
}
