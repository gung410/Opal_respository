using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public enum AddParticipantsCommandRegistrationAction
    {
        SaveAddedByCASuccessfully,
        SaveAddedByCAConflict,
        SaveAddedByCAClassfull
    }

    public class AddParticipantsCommand : BaseThunderCommand
    {
        public List<AddParticipantsCommandRegistrationItem> Items { get; set; }

        public Guid ClassRunId { get; set; }
    }

    public class AddParticipantsCommandRegistrationItem
    {
        public Guid? RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public AddParticipantsCommandRegistrationAction Action { get; set; }

        public List<Guid> OtherInProgressRegistrationIds { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
