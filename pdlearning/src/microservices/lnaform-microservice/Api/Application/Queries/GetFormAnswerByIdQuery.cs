using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormAnswerByIdQuery : BaseThunderQuery<FormAnswerModel>
    {
        public Guid UserId { get; set; }

        public Guid FormAnswerId { get; set; }
    }
}
