using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class DTDEntityRepository
    /// </summary>
    public class DTDEntityRepository : RepositoryBase<DTDEntity>, IDTDEntityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DTDEntityRepository" /> class.
        /// </summary>
        /// <param name="dbContext">OrganizationDbContext</param>
        public DTDEntityRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }

        public List<DTDEntity> GetDepartmentDepartmentTypes(
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> departmentTypeExtIds = null , bool includeDepartmentType = false)
        {
            var query = GenerateQuery(departmentIds, departmentTypeIds, departmentTypeExtIds, includeDepartmentType);
            return query.ToList();
        }
        public async Task<List<DTDEntity>> GetDepartmentDepartmentTypesAsync(
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> departmentTypeExtIds = null, bool includeDepartmentType = false)
        {
            var query = GenerateQuery(departmentIds, departmentTypeIds, departmentTypeExtIds, includeDepartmentType);
            return await query.ToListAsync();
        }

    

        public List<DTDEntity> GetDepartmentDepartmentTypesByDepartmentIds(List<int> departmentIds)
        {
            var query = GenerateQuery(departmentIds);
            return query.ToList();
        }
        public Task<List<DTDEntity>>  GetDepartmentDepartmentTypesByDepartmentIdsAsync(List<int> departmentIds)
        {
            var query = GenerateQuery(departmentIds);
            return query.ToListAsync();
        }

        private IQueryable<DTDEntity> GenerateQuery(List<int> departmentIds, List<int> departmentTypeIds,
            List<string> departmentTypeExtIds, bool includeDepartmentType)
        {
            var query = GetAllAsNoTracking();

            if (departmentTypeIds != null && departmentTypeIds.Any())
            {
                query = query.Where(t => departmentTypeIds.Contains(t.DepartmentTypeId));
            }

            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(t => departmentIds.Contains(t.DepartmentId));
            }

            if (departmentTypeExtIds != null && departmentTypeExtIds.Any())
            {
                query = query.Where(t => departmentTypeExtIds.Contains(t.DepartmentType.ExtId));
            }

            if (includeDepartmentType)
            {
                query = query.Include(d => d.DepartmentType);

            }


            return query;
        }

        private IQueryable<DTDEntity> GenerateQuery(List<int> departmentIds)
        {
            var query = GetAllAsNoTracking();
            if (departmentIds != null && departmentIds.Any())
            {
                if (departmentIds.Count == 1)
                {
                    var departmentId = departmentIds[0];
                    query = query.Where(t => t.DepartmentId == departmentId);
                }
                else
                {
                    query = query.Where(t => departmentIds.Contains(t.DepartmentId));
                }
            }

            return query;
        }
    }
}