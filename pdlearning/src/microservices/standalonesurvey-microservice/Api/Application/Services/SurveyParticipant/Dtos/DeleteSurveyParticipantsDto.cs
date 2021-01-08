using System;
using Microservice.StandaloneSurvey.Application.RequestDtos;

namespace Microservice.StandaloneSurvey.Application.Services.SurveyParticipant.Dtos
{
    public class DeleteSurveyParticipantsDto : HasSubModuleInfoBase
    {
        public Guid[] Ids { get; set; }

        public Guid FormId { get; set; }
    }
}
