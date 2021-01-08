using System.Collections.Generic;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Assessment.Data.Repositories.QueryBuilders;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    public class LevelGroupRepository : RepositoryBase<LevelGroupEntity>, ILevelGroupRepository
    {
        public LevelGroupRepository(AssessmentConfigContext assessmentConfigContext)
            : base(assessmentConfigContext)
        {
        }

        public List<LevelGroupEntity> GetLevelGroups(List<int> activityIds = null,
            List<int> levelGroupIds = null,
            List<int> departmentIds = null,
            List<int> roleIds = null,
            List<int> customerIds = null,
            List<string> tags = null,
            bool includeLocalizedData = false)
        {
            var query = GetAll();

            query = new LevelGroupQueryBuilder(query)
                .FilterByActivityIds(activityIds)
                .FilterByLevelGroupIds(levelGroupIds)
                .FilterByDepartmentIds(departmentIds)
                .FilterByRoleIds(roleIds)
                .FilterByCustomerIds(customerIds)
                .FilterByTags(tags)
                .Build();
            if (includeLocalizedData)
                query = query.Include(t => t.LT_LevelGroups);
            return query.ToList();
        }

        
    }
}
