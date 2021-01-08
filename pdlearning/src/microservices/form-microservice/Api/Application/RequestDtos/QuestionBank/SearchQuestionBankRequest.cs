using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Application.Queries;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.RequestDtos
{
    public class SearchQuestionBankRequest
    {
        public PagedResultRequestDto PagedInfo { get; set; }

        public string Title { get; set; }

        public IEnumerable<QuestionType> QuestionTypes { get; set; }

        public IEnumerable<Guid> QuestionGroupIds { get; set; }

        public SearchQuestionBankQuery ToQuery()
        {
            return new SearchQuestionBankQuery
            {
                PagedInfo = PagedInfo,
                Title = Title,
                QuestionGroupIds = QuestionGroupIds,
                QuestionTypes = QuestionTypes
            };
        }
    }
}
