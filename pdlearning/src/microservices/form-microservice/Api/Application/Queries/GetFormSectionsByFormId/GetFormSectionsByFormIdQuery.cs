using System;
using System.Collections.Generic;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormSectionsByFormIdQuery : BaseThunderQuery<List<FormSectionModel>>
    {
        public Guid FormId { get; set; }
    }
}
