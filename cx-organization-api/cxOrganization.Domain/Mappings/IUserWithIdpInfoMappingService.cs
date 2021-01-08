using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using System.Collections.Generic;

namespace cxOrganization.Domain.Mappings
{
    public interface IUserWithIdpInfoMappingService
    {
        UserWithIdpInfoDto ToUserDto(UserEntity userEntity, List<DTDEntity> dtdEntities, List<UGMemberEntity> ugMemberEntities);
    }
}
