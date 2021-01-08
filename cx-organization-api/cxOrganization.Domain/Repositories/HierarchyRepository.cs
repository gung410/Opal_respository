using System.Linq;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class HierarchyRepository
    /// </summary>
    public class HierarchyRepository : RepositoryBase<HierarchyEntity>, IHierarchyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public HierarchyRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public HierarchyDepartmentEntity GetH_DByHierarchyID(int hierarchyId,
            int departmentId = 0,
            bool includeDepartment = false,
            bool allowGetDepartmentDeleted = false)
        {
            IQueryable<HierarchyDepartmentEntity> hierarchyDepartment;
            if (allowGetDepartmentDeleted)
            {
                hierarchyDepartment = from hd in _dbContext.Set<HierarchyDepartmentEntity>()
                                      join d in _dbContext.Set<DepartmentEntity>() on hd.DepartmentId equals d.DepartmentId
                                      where hd.HierarchyId == hierarchyId && hd.DepartmentId == departmentId
                                      select hd;
            }
            else
            {
                hierarchyDepartment = from hd in _dbContext.Set<HierarchyDepartmentEntity>()
                                      join d in _dbContext.Set<DepartmentEntity>() on hd.DepartmentId equals d.DepartmentId
                                      where d.EntityStatusId == (short)EntityStatusEnum.Active && hd.HierarchyId == hierarchyId && hd.DepartmentId == departmentId && hd.Deleted == 0
                                      select hd;
            }
            if (includeDepartment)
                hierarchyDepartment = hierarchyDepartment.Include(x => x.Department);

            var hD = hierarchyDepartment.FirstOrDefault();
            return hD;
        }

    }
}
