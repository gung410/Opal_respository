using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Repositories
{
    public interface IDTDEntityRepository
    {
        List<DTDEntity> GetDepartmentDepartmentTypes(List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> departmentTypeExtIds = null, bool includeDepartmentType = false);

        Task<List<DTDEntity>> GetDepartmentDepartmentTypesAsync(List<int> departmentIds = null,
            List<int> departmentTypeIds = null,
            List<string> departmentTypeExtIds = null, bool includeDepartmentType = false);

        List<DTDEntity> GetDepartmentDepartmentTypesByDepartmentIds(List<int> departmentIds);
        Task<List<DTDEntity>> GetDepartmentDepartmentTypesByDepartmentIdsAsync(List<int> departmentIds);

    }
}