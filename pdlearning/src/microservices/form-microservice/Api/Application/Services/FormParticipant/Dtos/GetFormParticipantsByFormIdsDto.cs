using System;
using System.Collections.Generic;

namespace Microservice.Form.Application.Services.FormParticipant.Dtos
{
    public class GetFormParticipantsByFormIdsDto
    {
        public List<Guid> FormIds { get; set; }
    }
}
