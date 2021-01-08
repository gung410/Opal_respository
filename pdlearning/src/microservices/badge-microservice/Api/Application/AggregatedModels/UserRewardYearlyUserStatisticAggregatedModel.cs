using System.Collections.Generic;
using Microservice.Badge.Domain.Entities;

namespace Microservice.Badge.Application.AggregatedModels
{
    public class UserRewardYearlyUserStatisticAggregatedModel : UserRewardAggregatedModel
    {
        public IEnumerable<YearlyUserStatistic> YearlyUserStatistic { get; private set; }
    }
}
