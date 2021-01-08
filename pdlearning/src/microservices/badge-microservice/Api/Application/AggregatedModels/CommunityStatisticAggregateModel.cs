using System;
using System.Collections.Generic;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.AggregatedModels
{
    public class CommunityStatisticAggregateModel
    {
        public Guid CommunityId { get; set; }

        public IEnumerable<UserStatistic> UserStatistics { get; set; }
    }
}
