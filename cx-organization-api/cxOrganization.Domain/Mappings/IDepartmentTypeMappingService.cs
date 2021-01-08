using System.Collections.Generic;
using cxOrganization.Client.DepartmentTypes;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public interface IDepartmentTypeMappingService
    {
        IdentityStatusDto ToIdentityStatusDto(DepartmentTypeEntity departmentTypeEntity);
        List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtDepartmentTypeEntity> lTUserTypeEntities, string langcode = "");
        List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtDepartmentTypeEntity> lTUserTypeEntities, int languageId );

        DepartmentTypeDto ToDepartmentTypeDto(DepartmentTypeEntity departmentTypeEntity, string langCode = "en-US");
        DepartmentTypeDto ToDepartmentTypeDto(DepartmentTypeEntity departmentTypeEntity, int languageId);

       List<DepartmentTypeDto> ToDepartmentTypeDtos(List<DepartmentTypeEntity> departmentTypeEntities, string langCode = "en-US");
       List<DepartmentTypeDto> ToDepartmentTypeDtos(List<DepartmentTypeEntity> departmentTypeEntities, int languageId);
    }
}
