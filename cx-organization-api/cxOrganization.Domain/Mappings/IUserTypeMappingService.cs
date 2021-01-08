using System.Collections.Generic;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public interface IUserTypeMappingService
    {
        IdentityStatusDto ToIdentityStatusDto(UserTypeEntity userTypeEntity);

        List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtUserTypeEntity> lTUserTypeEntities, string langcode = "");
        List<LocalizedDataDto> ToLocalizedDataDto(IEnumerable<LtUserTypeEntity> lTUserTypeEntities, int languageId);
        UserTypeDto ToUserTypeDto(UserTypeEntity userTypeEntity, string langCode = "en-US");
        List<UserTypeDto> ToUserTypeDtos(ICollection<UserTypeEntity> userTypes, string langCode = "en-US");
        List<UserTypeDto> ToUserTypeDtos(ICollection<UserTypeEntity> userTypes, int languageId);
    }
}