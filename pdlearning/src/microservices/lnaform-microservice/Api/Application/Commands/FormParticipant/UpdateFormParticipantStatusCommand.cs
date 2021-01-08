using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class UpdateFormParticipantStatusCommand : BaseThunderCommand
    {
        public Guid FormId { get; set; }

        public Guid CurrentUserId { get; set; }
    }
}
