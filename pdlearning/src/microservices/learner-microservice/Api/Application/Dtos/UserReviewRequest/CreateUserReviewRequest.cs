using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class CreateUserReviewRequest
    {
        public Guid ItemId { get; set; }

        public int Rating { get; set; }

        public ItemType ItemType { get; set; }

        public string CommentContent { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
