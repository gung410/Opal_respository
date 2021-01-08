using cxPlatform.Core;
using cxOrganization.Domain.Entities;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IDepartmentTypeRepository
    /// </summary>
    public interface IDepartmentTypeRepository : IRepository<DepartmentTypeEntity>
    {
        List<DepartmentTypeEntity> GetDepartmentTypes(int ownerId,
            List<int> departmentIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null);

        List<DepartmentTypeEntity> GetAllDepartmentTypes(int ownerId,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false);
        Task<List<DepartmentTypeEntity>> GetAllDepartmentTypesAsync(int ownerId,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false);
        DepartmentTypeEntity GetDepartmentTypeByExtId(string extId, int? archetypeId = null);

        bool HasDepartmentType(int ownerId, int departmentId, int departmentTypeId);
        List<DepartmentTypeEntity> GetAllDepartmentTypesInCache();
        Task<List<DepartmentTypeEntity>> GetAllDepartmentTypesInCacheAsync();
        List<int> GetDepartmentTypeIds(List<string> departmentTypeExtIds);
        Task<List<int>> GetDepartmentTypeIdsAsync(List<string> departmentTypeExtIds);
    }
}
