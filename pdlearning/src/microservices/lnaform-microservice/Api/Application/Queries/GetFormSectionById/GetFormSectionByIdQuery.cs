using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormSectionByIdQuery : BaseThunderQuery<FormSectionModel>
    {
        public Guid Id { get; set; }
    }
}
