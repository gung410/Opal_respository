using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Domain.Entities;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface IEnsureYearlyStatisticExistLogic
    {
        Task<YearlyUserStatistic> ByYear(Guid userId, int year);

        Task<CommunityYearlyUserStatistic> ByYearAndCommunity(Guid userId, int year, Guid communityId);

        Task<List<CommunityYearlyUserStatistic>> ByYearAndCommunity(List<Guid> userIds, int year, Guid communityId, CancellationToken cancellationToken = default);
    }
}
