using System;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class CloneFormAsNewVersionCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public Guid NewId { get; set; }

        /// <summary>
        /// The Id of parent record.
        /// </summary>
        public Guid ParentId { get; set; }

        public FormStatus Status { get; set; }

        public string NewTitle { get; set; }

        /// <summary>
        /// In case clone to a new form, not to new version, set this property to False. Default is True.
        /// </summary>
        public bool IsCloneToNewVersion { get; set; } = true;
    }
}