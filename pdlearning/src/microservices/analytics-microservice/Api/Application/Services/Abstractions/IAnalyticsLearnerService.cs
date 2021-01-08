using System;
using System.Threading.Tasks;
using Microservice.Analytics.Domain.Entities;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Services.Abstractions
{
    public interface IAnalyticsLearnerService
    {
        Task CreateOrSetToDateBookmarkItem(
            Guid itemId,
            Guid userId,
            bool isUnBookmark,
            AnalyticLearnerBookmarkItemType itemType,
            DateTime time,
            SAM_UserHistory userHistory);
    }
}
