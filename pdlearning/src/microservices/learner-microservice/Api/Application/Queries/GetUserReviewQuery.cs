using System;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetUserReviewQuery : BaseThunderQuery<UserReviewModel>
    {
        public Guid ItemId { get; set; }

        public ItemType ItemType { get; set; }
    }
}
