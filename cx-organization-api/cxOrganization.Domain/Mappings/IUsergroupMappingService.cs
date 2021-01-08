using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public interface IUserGroupMappingService
    {
        ConexusBaseDto ToUserGroupDto(UserGroupEntity groupEntity, bool? getDynamicProperties = null);
        UserGroupEntity ToUserGroupEntity(UserGroupEntity groupEntity, UserGroupDtoBase groupDto);
        IdentityStatusDto ToIdentityStatusDto(UserGroupEntity department);
        MemberDto ToMemberDto(UserGroupEntity userGroup);
        TeachingSubjectDto ToTeachingSubjectDto(UserGroupEntity groupEntity);
    }
}
