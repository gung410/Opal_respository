using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetClassRunsByIdsQuery : BaseThunderQuery<IEnumerable<ClassRunModel>>
    {
        public IEnumerable<Guid> ClassRunIds { get; set; }
    }
}
