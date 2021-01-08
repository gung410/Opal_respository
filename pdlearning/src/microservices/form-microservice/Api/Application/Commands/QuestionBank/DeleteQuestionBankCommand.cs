using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class DeleteQuestionBankCommand : BaseThunderCommand
    {
        public Guid QuestionBankId { get; set; }
    }
}
