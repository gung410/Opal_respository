using System.Collections.Generic;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// IEventRepository
    /// </summary>
    public interface IActivityStatusTypeRepository : IRepository<ActivityStatusTypeEntity>
    {
        List<ActivityStatusTypeEntity> GetActivityStatusByActivityId(int activityID);
    }
}
