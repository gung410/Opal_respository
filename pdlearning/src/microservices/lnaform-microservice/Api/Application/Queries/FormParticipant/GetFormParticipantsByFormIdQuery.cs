using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormParticipantsByFormIdQuery : BaseThunderQuery<PagedResultDto<FormParticipantModel>>
    {
        public Guid FormOriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
