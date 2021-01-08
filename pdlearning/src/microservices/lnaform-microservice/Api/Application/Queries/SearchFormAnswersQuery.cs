using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class SearchFormAnswersQuery : BaseThunderQuery<PagedResultDto<FormAnswerModel>>
    {
        public Guid UserId { get; set; }

        public Guid FormId { get; set; }

        public Guid? ResourceId { get; set; }

        public bool? IsSubmitted { get; set; }

        public bool? IsCompleted { get; set; }

        public bool? BeforeDueDate { get; set; }

        public bool? BeforeTimeLimit { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
