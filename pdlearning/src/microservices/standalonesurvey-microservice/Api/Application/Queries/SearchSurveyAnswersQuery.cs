using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchSurveyAnswersQuery : BaseStandaloneSurveyQuery<PagedResultDto<SurveyAnswerModel>>
    {
        public Guid UserId { get; set; }

        public Guid SurveyId { get; set; }

        public Guid? ResourceId { get; set; }

        public bool? IsSubmitted { get; set; }

        public bool? IsCompleted { get; set; }

        public bool? BeforeDueDate { get; set; }

        public bool? BeforeTimeLimit { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
