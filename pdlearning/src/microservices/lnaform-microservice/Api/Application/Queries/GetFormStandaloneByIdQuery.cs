using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormStandaloneByIdQuery : BaseThunderQuery<FormWithQuestionsModel>
    {
        public Guid FormOriginalObjectId { get; set; }

        public Guid UserId { get; set; }
    }
}
