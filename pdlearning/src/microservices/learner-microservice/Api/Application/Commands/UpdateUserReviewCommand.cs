using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class UpdateUserReviewCommand : BaseThunderCommand
    {
        public Guid ItemId { get; set; }

        public ItemType ItemType { get; set; }

        public int Rating { get; set; }

        public string CommentContent { get; set; }
    }
}
