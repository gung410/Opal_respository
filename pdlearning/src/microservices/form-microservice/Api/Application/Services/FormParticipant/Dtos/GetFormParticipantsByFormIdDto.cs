using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.Services.FormParticipant.Dtos
{
    public class GetFormParticipantsByFormIdDto
    {
        public Guid FormOriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
