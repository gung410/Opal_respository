using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface IGeneralBadgeCriteriaResolverLogic<TCriteria, TResult>
    {
        Task<IAggregateFluent<TResult>> GetQualifiedUserAsync(DateTime statisticDatetime);
    }
}
