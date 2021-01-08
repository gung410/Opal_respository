using System;
using System.Collections.Generic;
using Microservice.Badge.Application.Models;

namespace Microservice.Badge.Application.AggregatedModels
{
    public class UserRewardAggregatedModel
    {
        public Guid Id { get; set; }

        public IEnumerable<UserBadgeModel> UserRewards { get; set; }
    }
}
