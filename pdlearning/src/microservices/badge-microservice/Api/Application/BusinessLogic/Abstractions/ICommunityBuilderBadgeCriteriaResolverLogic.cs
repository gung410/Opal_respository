using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Badge.Application.AggregatedModels;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface ICommunityBuilderBadgeCriteriaResolverLogic<TCriteria>
    {
        Task<List<IAggregateFluent<CommunityStatisticAggregateModel>>> GetQualifiedUserAsync(DateTime statisticDatetime);
    }
}
