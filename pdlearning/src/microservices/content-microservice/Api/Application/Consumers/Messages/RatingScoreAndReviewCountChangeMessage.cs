using System;

namespace Microservice.Content.Application.Consumers
{
    public class RatingScoreAndReviewCountChangeMessage
    {
        public Guid ItemId { get; set; }

        public double AverageRating { get; set; }

        public int ReviewsCount { get; set; }
    }
}
