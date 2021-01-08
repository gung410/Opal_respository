using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Commands
{
    public class DeletePersonalFileCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }
    }
}
