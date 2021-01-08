using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormByIdQuery : BaseThunderQuery<FormModel>
    {
        public Guid UserId { get; set; }

        public Guid FormId { get; set; }
    }
}
