using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormWithQuestionsByIdQuery : BaseThunderQuery<FormWithQuestionsModel>
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
