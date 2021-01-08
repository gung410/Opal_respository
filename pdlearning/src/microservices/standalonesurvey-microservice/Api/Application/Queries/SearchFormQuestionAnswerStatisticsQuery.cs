using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchFormQuestionAnswerStatisticsQuery : BaseStandaloneSurveyQuery<PagedResultDto<SurveyQuestionAnswerStatisticsModel>>
    {
        public Guid? FormAnswerId { get; set; }

        public Guid? FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
