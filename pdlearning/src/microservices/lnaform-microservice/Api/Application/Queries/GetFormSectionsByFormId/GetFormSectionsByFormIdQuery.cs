using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormSectionsByFormIdQuery : BaseThunderQuery<List<FormSectionModel>>
    {
        public Guid FormId { get; set; }
    }
}
