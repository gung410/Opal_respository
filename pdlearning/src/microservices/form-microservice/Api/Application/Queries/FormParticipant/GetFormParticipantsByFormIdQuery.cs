using System;
using System.Collections.Generic;
using Microservice.Form.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormParticipantsByFormIdQuery : BaseThunderQuery<PagedResultDto<FormParticipantModel>>
    {
        public Guid FormOriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
