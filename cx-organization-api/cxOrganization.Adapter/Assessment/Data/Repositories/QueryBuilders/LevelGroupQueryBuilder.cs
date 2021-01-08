using System.Collections.Generic;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Shared.Extensions;

namespace cxOrganization.Adapter.Assessment.Data.Repositories.QueryBuilders
{
    public class LevelGroupQueryBuilder
    {
        IQueryable<LevelGroupEntity> _query;
        public LevelGroupQueryBuilder(IQueryable<LevelGroupEntity> query)
        {
            _query = query;
        }
        public IQueryable<LevelGroupEntity> Build()
        {
            return _query;
        }
        public LevelGroupQueryBuilder FilterByTags(List<string> tags)
        {
            if (tags.IsNotNullOrEmpty())
            {
                if (tags.Count == 1)
                {
                    string value = tags[0];
                    _query = _query.Where(t => t.Tag == value);
                }
                else
                {
                    _query = _query.Where(t => tags.Contains(t.Tag));
                }
            }

            return this;
        }

        public LevelGroupQueryBuilder FilterByCustomerIds(List<int> customerIds)
        {
            if (customerIds.IsNotNullOrEmpty())
            {
                if (customerIds.Count == 1)
                {
                    int value = customerIds[0];
                    _query = _query.Where(t => t.CustomerId == value || t.CustomerId == null);
                }
                else
                {
                    _query = _query.Where(t => customerIds.Contains(t.CustomerId.Value));
                }
            }

            return this;
        }

        public LevelGroupQueryBuilder FilterByRoleIds(List<int> roleIds)
        {
            if (roleIds.IsNotNullOrEmpty())
            {
                if (roleIds.Count == 1)
                {
                    int value = roleIds[0];
                    _query = _query.Where(t => t.RoleId == value);
                }
                else
                {
                    _query = _query.Where(t => roleIds.Contains(t.RoleId.Value));
                }
            }

            return this;
        }

        public LevelGroupQueryBuilder FilterByDepartmentIds(List<int> departmentIds)
        {
            if (departmentIds.IsNotNullOrEmpty())
            {
                if (departmentIds.Count == 1)
                {
                    int value = departmentIds[0];
                    _query = _query.Where(t => t.DepartmentId == value);
                }
                else
                {
                    _query = _query.Where(t => departmentIds.Contains(t.DepartmentId.Value));
                }
            }

            return this;
        }

        public LevelGroupQueryBuilder FilterByLevelGroupIds(List<int> levelgroupIds)
        {
            if (levelgroupIds.IsNotNullOrEmpty())
            {
                if (levelgroupIds.Count == 1)
                {
                    int value = levelgroupIds[0];
                    _query = _query.Where(t => t.LevelGroupId == value);
                }
                else
                {
                    _query = _query.Where(t => levelgroupIds.Contains(t.LevelGroupId));
                }
            }

            return this;
        }

        public LevelGroupQueryBuilder FilterByActivityIds(List<int> activityIds)
        {
            if (activityIds.IsNotNullOrEmpty())
            {
                if (activityIds.Count == 1)
                {
                    int value = activityIds[0];
                    _query = _query.Where(t => t.ActivityId == value);
                }
                else
                {
                    _query = _query.Where(t => activityIds.Contains(t.ActivityId));
                }
            }

            return this;
        }
    }
}
