using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormByIdQuery : BaseThunderQuery<FormModel>
    {
        public Guid UserId { get; set; }

        public Guid FormId { get; set; }
    }
}
