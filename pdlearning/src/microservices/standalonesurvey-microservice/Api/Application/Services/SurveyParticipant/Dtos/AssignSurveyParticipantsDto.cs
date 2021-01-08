using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.RequestDtos;

namespace Microservice.StandaloneSurvey.Application.Services.SurveyParticipant.Dtos
{
    public class AssignSurveyParticipantsDto : HasSubModuleInfoBase
    {
        public List<Guid> UserIds { get; set; }

        public Guid FormOriginalObjectId { get; set; }

        public Guid FormId { get; set; }
    }
}
