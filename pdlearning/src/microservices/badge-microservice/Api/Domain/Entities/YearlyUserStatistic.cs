using System;
using System.Linq.Expressions;
using Microservice.Badge.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservice.Badge.Domain.Entities
{
    /// <summary>
    /// This entity defines activity statistic of users each year.
    /// It also contains detail statistic of user each month, each day for ensuring ability to track data for analytic.
    /// </summary>
    public class YearlyUserStatistic : BaseYearlyUserStatistic<GeneralStatistic>
    {
        public YearlyUserStatistic()
        {
        }

        public YearlyUserStatistic(Guid userId, int year)
        {
            this.UserId = userId;
            this.Year = year;
        }

        [BsonElement]
        public int SumOfLatestDailyPostAndLike => LatestDailyStatistic == null ? 0 : LatestDailyStatistic.NumOfPost + LatestDailyStatistic.NumOfLikePost;

        public static Expression<Func<YearlyUserStatistic, bool>> MatchedCollaborativeLearnersBadgeCriteria(
            CollaborativeLearnersBadgeCriteria collaborativeLearnersBadgeCriteria)
        {
            return p => p.LatestDailyStatistic.NumOfForward >= collaborativeLearnersBadgeCriteria.SumOfForward &&
                        p.LatestDailyStatistic.NumOfFollowCommunity >= collaborativeLearnersBadgeCriteria.SumOfFollow &&
                        p.LatestDailyStatistic.NumOfPostResponding >= collaborativeLearnersBadgeCriteria.SumOfPostsResponded &&
                        p.SumOfLatestDailyPostAndLike >= collaborativeLearnersBadgeCriteria.SumOfPostAndLike;
        }

        public static Expression<Func<YearlyUserStatistic, bool>> MatchedReflectiveLearnersBadgeCriteria(
            ReflectiveLearnersBadgeCriteria reflectiveLearnersBadgeCriteria)
        {
            return p => p.LatestMonthlyStatistics.NumOfSharedReflection > reflectiveLearnersBadgeCriteria.SumOfSharedReflection &&
            p.LatestMonthlyStatistics.NumOfReflection > reflectiveLearnersBadgeCriteria.SumOfReflection;
        }

        public static Expression<Func<YearlyUserStatistic, bool>> MatchedDigitalLearnersBadgeCriteria(
            DigitalLearnersBadgeCriteria digitalLearnersBadgeCriteria)
        {
            return p => p.LatestMonthlyStatistics.NumOfCompletedDigitalResources >= digitalLearnersBadgeCriteria.NumOfCompletedDigitalResources &&
                        p.LatestMonthlyStatistics.NumOfCompletedElearning >= digitalLearnersBadgeCriteria.NumOfCompletedElearning &&
                        p.LatestMonthlyStatistics.NumOfCompletedMLU >= digitalLearnersBadgeCriteria.NumOfCompletedMLU;
        }
    }
}
