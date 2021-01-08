using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormParticipantsByFormIdQuery : BaseStandaloneSurveyQuery<PagedResultDto<SurveyParticipantModel>>
    {
        public Guid FormOriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
