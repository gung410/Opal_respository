using System;
using Microservice.Content.Versioning.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Application.Commands
{
    public class RevertVersionCommand : BaseThunderCommand
    {
        public RevertVersionRequest Request { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// This ID will be ID of new record when execute a clone command.
        /// </summary>
        public Guid NewActiveId { get; set; }
    }
}
