using System;
using Microservice.LnaForm.Versioning.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Versioning.Application.Commands
{
    public class SaveVersionTrackingComand : BaseThunderCommand
    {
        public CreateVersionTrackingRequest CreationRequest { get; set; }

        public bool IsIncreaseMajorVersion { get; set; }

        public bool IsIncreaseMinorVersion { get; set; }

        public Guid UserId { get; set; }
    }
}
