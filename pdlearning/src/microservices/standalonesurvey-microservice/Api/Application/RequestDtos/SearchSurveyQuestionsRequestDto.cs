using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class SearchSurveyQuestionsRequestDto : HasSubModuleInfoBase
    {
        public Guid FormId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
