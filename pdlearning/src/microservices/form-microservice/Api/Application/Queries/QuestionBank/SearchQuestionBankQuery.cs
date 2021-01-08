using System;
using System.Collections.Generic;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class SearchQuestionBankQuery : BaseThunderQuery<PagedResultDto<QuestionBankModel>>
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public string Title { get; set; }

        public IEnumerable<QuestionType> QuestionTypes { get; set; }

        public IEnumerable<Guid> QuestionGroupIds { get; set; }
    }
}
