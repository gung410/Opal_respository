using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetNoOfAssessmentDonesQuery : BaseThunderQuery<IEnumerable<NoOfAssessmentDoneInfoModel>>
    {
        public IEnumerable<Guid> ParticipantAssignmentTrackIds { get; set; }
    }
}
