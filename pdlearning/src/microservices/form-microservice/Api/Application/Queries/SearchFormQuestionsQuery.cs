using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class SearchFormQuestionsQuery : BaseThunderQuery<PagedResultDto<FormQuestionModel>>
    {
        public Guid UserId { get; set; }

        public Guid FormId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
