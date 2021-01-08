using System;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.Services.SurveyParticipant.Dtos
{
    public class GetSurveyParticipantsByFormIdDto : HasSubModuleInfoBase
    {
        public Guid FormOriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
