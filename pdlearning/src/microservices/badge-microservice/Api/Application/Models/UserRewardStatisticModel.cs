using System;
using System.Collections.Generic;

namespace Microservice.Badge.Application.Models
{
    public enum YearlyUserStatisticType
    {
        None,
        LatestDaily,
        LatestMonthly,
        Yearly
    }

    public class UserRewardStatisticModel
    {
        public bool Awarded { get; init; }

        public List<UserBadgeModel> AwardedBadges { get; set; }

        public Guid UserId { get; init; }

        public YearlyUserStatisticType Type { get; init; }

        public UserStatisticModel Statistic { get; set; }

        public string Id => UserId.ToString() + '_' + Statistic?.Year + '_' + Type;
    }
}
