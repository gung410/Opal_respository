using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetAllResponseQuery : BaseStandaloneSurveyQuery<PagedResultDto<ResponsesModel>>
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public Guid FormId { get; set; }
    }
}
