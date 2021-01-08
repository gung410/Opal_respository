using System;
using System.Collections.Generic;
using cxPlatform.Core;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IHierarchyRepository
    /// </summary>
    public interface IHierarchyRepository : IRepository<HierarchyEntity>
    {
        HierarchyDepartmentEntity GetH_DByHierarchyID(int hierarchyId, int departmentId = 0, bool includeDepartment = false, bool allowGetDepartmentDeleted = false);
    }
}

