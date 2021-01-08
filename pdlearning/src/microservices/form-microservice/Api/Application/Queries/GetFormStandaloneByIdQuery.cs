using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormStandaloneByIdQuery : BaseThunderQuery<FormWithQuestionsModel>
    {
        public Guid FormOriginalObjectId { get; set; }

        public Guid UserId { get; set; }
    }
}
