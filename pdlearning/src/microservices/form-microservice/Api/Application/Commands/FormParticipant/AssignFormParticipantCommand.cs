using System;
using System.Collections.Generic;
using Microservice.Form.Application.Services;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
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
