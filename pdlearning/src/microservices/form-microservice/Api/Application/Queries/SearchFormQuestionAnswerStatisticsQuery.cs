using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class SearchFormQuestionAnswerStatisticsQuery : BaseThunderQuery<PagedResultDto<FormQuestionAnswerStatisticsModel>>
    {
        public Guid? FormAnswerId { get; set; }

        public Guid? FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public double? MaxScore { get; set; }

        public double? Score { get; set; }

        public Guid? ScoredBy { get; set; }

        public string AnswerFeedback { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public int? SpentTimeInSeconds { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
