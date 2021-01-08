using System;
using Microservice.Content.Versioning.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Application.Commands
{
    public class SaveVersionTrackingComand : BaseThunderCommand
    {
        public CreateVersionTrackingRequest CreationRequest { get; set; }

        public bool IsIncreaseMajorVersion { get; set; }

        public bool IsIncreaseMinorVersion { get; set; }

        public Guid UserId { get; set; }
    }
}
