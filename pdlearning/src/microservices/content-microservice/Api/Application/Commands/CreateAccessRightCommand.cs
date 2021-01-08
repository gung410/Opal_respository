using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class CreateAccessRightCommand : BaseThunderCommand
    {
        public CreateAccessRightCommand(Guid? id, List<Guid> userIds, Guid originalObjectId, Guid userId)
        {
            Id = id;
            UserIds = userIds;
            OriginalObjectId = originalObjectId;
            UserId = userId;
        }

        public Guid? Id { get; }

        public List<Guid> UserIds { get; }

        public Guid OriginalObjectId { get; }

        public Guid UserId { get; }
    }
}
