using System;
using System.Collections.Generic;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class RemindSurveyParticipantRequest : HasSubModuleInfoBase
    {
        public List<Guid> ParticipantIds { get; set; }

        public Guid FormId { get; set; }
    }
}
