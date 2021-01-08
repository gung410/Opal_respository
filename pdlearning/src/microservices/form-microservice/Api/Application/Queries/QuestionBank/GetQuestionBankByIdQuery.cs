using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetQuestionBankByIdQuery : BaseThunderQuery<QuestionBankModel>
    {
        public Guid QuestionBankId { get; set; }
    }
}
