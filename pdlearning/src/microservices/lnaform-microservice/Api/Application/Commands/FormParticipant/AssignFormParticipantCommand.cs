using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class AssignFormParticipantCommand : BaseThunderCommand
    {
        public List<Guid> UserIds { get; set; }

        public Guid FormOriginalObjectId { get; set; }

        public Guid FormId { get; set; }

        public bool? IsStarted { get; set; }

        public FormParticipantStatus? Status { get; set; }

        public Guid CurrentUserId { get; set; }
    }
}
