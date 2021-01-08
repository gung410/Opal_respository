using System;
using System.Collections.Generic;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormParticipantsByFormIdsQuery : BaseThunderQuery<IEnumerable<FormParticipantFormModel>>
    {
        public List<Guid> FormIds { get; set; }
    }
}
