using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public interface IUserMappingService
    {
        int? UserTypeId { get; }

        ConexusBaseDto ToUserDto(UserEntity userEntity,
            bool? getDynamicProperties = null,
            bool keepEncryptedSsn = false, bool keepDecryptedSsn = false,
            List<UGMemberEntity> ugMemberEntities = null);
        UserEntity ToUserEntity(DepartmentEntity parentDepartment,
            HierarchyDepartmentEntity parentHd,
            UserEntity entity,
            UserDtoBase userDtobool,
            bool skipCheckingEntityVersion = false,
            int? currentOwnerId = null);
        IdentityStatusDto ToIdentityStatusDto(UserEntity user);
        MemberDto ToMemberDto(UserEntity user);
        UserEntity ToUserEntity(DepartmentEntity parentDepartment,UserEntity entity, UserDtoBase userDto, int? currentOwnerId = null);

        void HideOrDecryptSSN(UserDtoBase userDto);
    }
}
