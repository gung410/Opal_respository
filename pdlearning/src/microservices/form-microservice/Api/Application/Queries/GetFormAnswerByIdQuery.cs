using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormAnswerByIdQuery : BaseThunderQuery<FormAnswerModel>
    {
        public Guid UserId { get; set; }

        public Guid FormAnswerId { get; set; }
    }
}
