using System;
using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Application.Commands
{
    public class CloneDigitalContentAsNewVersionCommand : CloneDigitalContentCommand
    {
        /// <summary>
        /// The Id of parent record.
        /// </summary>
        public Guid ParentId { get; set; }

        public DigitalContentStatus Status { get; set; }
    }
}
