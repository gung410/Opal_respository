using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyDigitalContentByIdsQuery : BaseThunderQuery<List<DigitalContentModel>>
    {
        public IEnumerable<Guid> DigitalContentIds { get; set; }
    }
}
