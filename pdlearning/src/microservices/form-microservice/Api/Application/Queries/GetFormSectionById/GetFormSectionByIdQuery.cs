using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormSectionByIdQuery : BaseThunderQuery<FormSectionModel>
    {
        public Guid Id { get; set; }
    }
}
