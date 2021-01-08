using System;

namespace Microservice.Form.Application.Services.FormParticipant.Dtos
{
    public class DeleteFormParticipantsDto
    {
        public Guid[] Ids { get; set; }

        public Guid FormId { get; set; }
    }
}
