using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetClassRunRemainingSlotQuery : BaseThunderQuery<Dictionary<Guid, int>>
    {
        public IEnumerable<Guid> ClassRunIds { get; set; }
    }
}
