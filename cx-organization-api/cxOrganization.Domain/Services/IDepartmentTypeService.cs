using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Client.DepartmentTypes;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IDepartmentTypeService
    {
        List<IdentityStatusDto> GetDepartmentTypes(int ownerId,
            List<int> departmentIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null);

        List<DepartmentTypeDto> GetAllDepartmentTypes(int ownerId,
            List<ArchetypeEnum> archetypeIds,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false,
            string langCode = "en-US");
        Task<List<DepartmentTypeDto>> GetAllDepartmentTypesAsync(int ownerId,
            List<ArchetypeEnum> archetypeIds,
            List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false,
            string langCode = "en-US");

        DepartmentTypeEntity GetDepartmentTypeByExtId(string extId, int? archeTypeId = null);

        bool HasDepartmentType(int ownerId, int departmentId, int departmentTypeId);
    }
}
