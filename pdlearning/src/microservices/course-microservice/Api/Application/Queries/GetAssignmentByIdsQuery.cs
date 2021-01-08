using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAssignmentByIdsQuery : BaseThunderQuery<IEnumerable<AssignmentModel>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public bool IncludeQuizForm { get; set; }
    }
}
