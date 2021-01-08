using System;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class RollbackFormCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public Guid RevertFromRecordId { get; set; }

        public Guid RevertToRecordId { get; set; }

        public Guid CurrentActiveId { get; set; }
    }
}
