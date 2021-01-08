using System.Collections.Generic;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    public interface ILevelLimitRepository : IRepository<LevelLimitEntity>
    {
        List<LevelLimitEntity> GetLevelLimits(List<int> activityIds = null,
            List<int> levelLimitIds = null,
            List<int> levelGroupIds = null,
            List<int> categoryIds = null,
            List<int> questionIds = null,
            List<int> itemIds = null,
            List<string> levelGroupTags = null,
            bool includeLocalizedData = false);
    }
}
