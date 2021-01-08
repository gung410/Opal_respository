using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// IEventRepository
    /// </summary>
    public interface IActivityRepository : IRepository<ActivityEntity>
    {
        List<ActivityEntity> GetActivityByExternalId(string activityExtId);
        List<ActivityEntity> GetActivitiesByOwner(int ownerid);
        List<ActivityEntity> GetActivities(int ownerId = 0,
            List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null);
        Task<List<ActivityEntity>> GetActivitiesAsync(int ownerId = 0,
            List<int> surveyIds = null,
            List<int> activityIds = null,
            List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null);
        ActivityEntity GetActivityById(int activityId);
    }
}
