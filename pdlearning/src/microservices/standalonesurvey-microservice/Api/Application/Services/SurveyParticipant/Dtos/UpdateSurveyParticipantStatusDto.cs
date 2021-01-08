using System;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;

namespace Microservice.StandaloneSurvey.Application.Services.SurveyParticipant.Dtos
{
    public class UpdateSurveyParticipantStatusDto : HasSubModuleInfoBase
    {
        public Guid FormId { get; set; }

        public SurveyParticipantStatus? Status { get; set; }
    }
}
