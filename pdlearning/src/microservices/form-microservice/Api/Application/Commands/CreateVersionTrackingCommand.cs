using System;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class CreateVersionTrackingCommand : BaseThunderCommand
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public FormStatus FormStatus { get; set; }
    }
}
