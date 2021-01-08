namespace Microservice.Learner.Application.Common
{
    public class ReviewSummary
    {
        public ReviewSummary(int reviewCount, double averageRating)
        {
            ReviewCount = reviewCount;
            AverageRating = averageRating;
        }

        public int ReviewCount { get; }

        public double AverageRating { get; }

        public static ReviewSummary Default()
        {
            return new ReviewSummary(0, 0);
        }
    }
}
