using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class GetUserReviewByCurrentUserRequest
    {
        public Guid ItemId { get; set; }

        public ItemType ItemType { get; set; }
    }
}
