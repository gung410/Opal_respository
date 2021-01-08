using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class CreateUserReviewCommand : BaseThunderCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Guid ItemId { get; set; }

        public ItemType ItemType { get; set; }

        public string UserFullName { get; set; }

        public int Rating { get; set; }

        public string CommentContent { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
