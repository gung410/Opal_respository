using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class SearchFormQuestionAnswerStatisticsQuery : BaseThunderQuery<PagedResultDto<FormQuestionAnswerStatisticsModel>>
    {
        public Guid? FormAnswerId { get; set; }

        public Guid? FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
