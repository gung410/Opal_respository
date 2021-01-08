using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetSessionsByClassRunIdsQuery : BaseThunderQuery<IEnumerable<SessionModel>>
    {
        public IEnumerable<Guid> ClassRunIds { get; set; }
    }
}
