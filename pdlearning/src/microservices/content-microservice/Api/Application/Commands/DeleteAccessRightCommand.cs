using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class DeleteAccessRightCommand : BaseThunderCommand
    {
        public DeleteAccessRightCommand(Guid accessRightId)
        {
            AccessRightId = accessRightId;
        }

        public Guid AccessRightId { get; }
    }
}
