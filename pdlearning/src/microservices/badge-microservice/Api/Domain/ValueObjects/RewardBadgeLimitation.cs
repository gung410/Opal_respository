using System.Collections.Generic;
using Microservice.Badge.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Microservice.Badge.Domain.ValueObjects
{
    /// <summary>
    /// Define for issuing limitation if there are too many learners that archived the criteria to win a badge.
    /// </summary>
    public class RewardBadgeLimitation
    {
        // Do not allow initialize manually. Use static methods instead.
        public RewardBadgeLimitation()
        {
        }

        public RewardBadgeLimitType LimitType { get; init; }

        public int MaximumIsusedPeople { get; init; } = int.MaxValue;

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
        public Dictionary<RewardBadgeLimitType, int> LimitValues { get; init; }

        /// <summary>
        /// Initializes a new instance of <see cref="RewardBadgeLimitation"/> for the limit type <see cref="RewardBadgeLimitType.TopPercent"/>.
        /// </summary>
        /// <param name="percentValue">Number of percent people can be issued.</param>
        /// <returns>a new instance of <see cref="RewardBadgeLimitation"/>.</returns>
        public static RewardBadgeLimitation NewPercentBadgeLimitation(int percentValue)
        {
            return new()
            {
                LimitType = RewardBadgeLimitType.TopPercent,
                LimitValues = new Dictionary<RewardBadgeLimitType, int> { { RewardBadgeLimitType.TopPercent, percentValue } }
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RewardBadgeLimitation"/> for the limit type <see cref="RewardBadgeLimitType.MaximumPeople"/>.
        /// </summary>
        /// <param name="maximumPeople">Maximum people can be issued.</param>
        /// <param name="maximumIsusedPeople">Maximum issued people can award.</param>
        /// <returns>a new instance of <see cref="RewardBadgeLimitation"/>.</returns>
        public static RewardBadgeLimitation NewMaximumPeopleBadgeLimitation(int maximumPeople, int maximumIsusedPeople = int.MaxValue)
        {
            return new()
            {
                LimitType = RewardBadgeLimitType.MaximumPeople,
                MaximumIsusedPeople = maximumIsusedPeople,
                LimitValues = new Dictionary<RewardBadgeLimitType, int> { { RewardBadgeLimitType.MaximumPeople, maximumPeople } }
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RewardBadgeLimitation"/> for the limit type <see cref="RewardBadgeLimitType.MinOfEitherTopPercentOrMaximumPeople"/>.
        /// </summary>
        /// <param name="maximumPeople">Maximum people can be issued.</param>
        /// <param name="percentValue">Number of percent people can be issued.</param>
        /// <returns>a new instance of <see cref="RewardBadgeLimitation"/>.</returns>
        public static RewardBadgeLimitation NewCombinedBadgeLimitation(int maximumPeople, int percentValue)
        {
            return new()
            {
                LimitType = RewardBadgeLimitType.MinOfEitherTopPercentOrMaximumPeople,
                LimitValues = new Dictionary<RewardBadgeLimitType, int>
                {
                    { RewardBadgeLimitType.MaximumPeople, maximumPeople },
                    { RewardBadgeLimitType.TopPercent, percentValue }
                }
            };
        }
    }
}
