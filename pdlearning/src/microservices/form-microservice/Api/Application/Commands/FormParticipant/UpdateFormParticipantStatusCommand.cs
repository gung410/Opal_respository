using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class UpdateFormParticipantStatusCommand : BaseThunderCommand
    {
        public Guid FormId { get; set; }

        public Guid CurrentUserId { get; set; }
    }
}
