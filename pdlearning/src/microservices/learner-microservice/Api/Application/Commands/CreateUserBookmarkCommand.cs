using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class CreateUserBookmarkCommand : BaseThunderCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Guid ItemId { get; set; }

        public BookmarkType ItemType { get; set; }
    }
}
