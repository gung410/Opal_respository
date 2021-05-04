using cxOrganization.Client;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Validators
{
    public interface IUserGroupValidator
    {
        UserGroupEntity Validate(ConexusBaseDto dto, IAdvancedWorkContext workContext = null);
        UserGroupEntity Validate(int userGroupId);
        void ValidateMember(MemberDto member);
        UserEntity ValidateMemberDto(int userGroupId, MemberDto member, ref UserGroupEntity userGroupEntity);
        UserGroupEntity ValidateUserGroupDto(int userGroupId);
        UserGroupEntity ValidateMembership(MemberDto memberDto);
    }
}
