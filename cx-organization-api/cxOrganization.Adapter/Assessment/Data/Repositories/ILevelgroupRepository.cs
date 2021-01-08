using System.Collections.Generic;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    public interface ILevelGroupRepository : IRepository<LevelGroupEntity>
    {
        List<LevelGroupEntity> GetLevelGroups(List<int> activityIds = null,
            List<int> levelgroupIds = null,
            List<int> departmentIds = null,
            List<int> roleIds = null,
            List<int> customerIds = null,
            List<string> tags = null,
            bool includeLocalizedData = false);
    }
}
