using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class ArchiveDigitalContentCommand : BaseThunderCommand
    {
        public Guid ContentId { get; set; }

        public Guid? ArchiveBy { get; set; }
    }
}
