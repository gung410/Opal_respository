using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class AverageReviewRatingModel
    {
        public Guid ItemId { get; set; }

        public double AverageRating { get; set; }

        public int ReviewsCount { get; set; }

        public ItemType ItemType { get; set; }
    }
}
