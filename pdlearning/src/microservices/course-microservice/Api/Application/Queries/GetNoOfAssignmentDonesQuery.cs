using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetNoOfAssignmentDonesQuery : BaseThunderQuery<IEnumerable<NoOfAssignmentDoneInfoModel>>
    {
        public IEnumerable<Guid> RegistrationIds { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
