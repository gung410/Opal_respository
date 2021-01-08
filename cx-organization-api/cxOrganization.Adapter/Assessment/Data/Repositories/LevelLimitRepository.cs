using System.Collections.Generic;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Assessment.Data.Repositories.QueryBuilders;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    public class LevelLimitRepository : RepositoryBase<LevelLimitEntity>, ILevelLimitRepository
    {
        public LevelLimitRepository(AssessmentConfigContext unitOfWork)
            : base(unitOfWork)
        {
        }
        public List<LevelLimitEntity> GetLevelLimits(
            List<int> activityIds = null,
            List<int> levelLimitIds = null,
            List<int> levelGroupIds = null,
            List<int> categoryIds = null,
            List<int> questionIds = null,
            List<int> itemIds = null,
            List<string> levelGroupTags = null,
            bool includeLocalizedData = false)
        {

            var query = GetAll();
            query = new LevelLimitQueryBuilder(query)
                        .FilterByActivityIds(activityIds)
                        .FilterByLevelGroupTags(levelGroupTags)
                        .FilterByItemIds(itemIds)
                        .FilterByQuestionIds(questionIds)
                        .FilterByCategoryIds(categoryIds)
                        .FilterByLevelLimitIds(levelLimitIds)
                        .FilterByLevelGroupIds(levelGroupIds)
                        .Build();
            query = query.Include(t => t.OwnerColor);
            if (includeLocalizedData)
                query = query.Include(t => t.LT_LevelLimits);
            return query.ToList();
        }
    }
}
