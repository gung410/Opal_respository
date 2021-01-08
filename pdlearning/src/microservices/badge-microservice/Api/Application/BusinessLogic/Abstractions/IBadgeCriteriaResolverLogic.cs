using System;
using Microservice.Badge.Application.Models;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface IBadgeCriteriaResolverLogic<in TCriteria, TResult>
    {
        YearlyUserStatisticType YearlyUserStatisticType { get; }

        IAggregateFluent<TResult> ResolveCriteriaAsync(
            TCriteria badgeCriteria,
            DateTime statisticDatetime);
    }
}
