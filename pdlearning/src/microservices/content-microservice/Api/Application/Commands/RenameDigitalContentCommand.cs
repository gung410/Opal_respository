using System;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class RenameDigitalContentCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public RenameDigitalContentRequest Request { get; set; }
    }
}
