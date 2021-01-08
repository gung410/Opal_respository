namespace Microservice.Badge.Domain.Enums
{
    public enum RewardBadgeLimitType
    {
        /// <summary>
        /// Top percent can be issued.
        /// </summary>
        TopPercent,

        /// <summary>
        /// Maximum people can be issued.
        /// </summary>
        MaximumPeople,

        /// <summary>
        /// Combine <see cref="TopPercent"/> and <see cref="MaximumPeople"/> to get fewer learners can be issued the badge.
        /// </summary>
        MinOfEitherTopPercentOrMaximumPeople
    }
}
