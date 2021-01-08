using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchFormQuestionsQuery : BaseStandaloneSurveyQuery<PagedResultDto<SurveyQuestionModel>>
    {
        public Guid UserId { get; set; }

        public Guid FormId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
