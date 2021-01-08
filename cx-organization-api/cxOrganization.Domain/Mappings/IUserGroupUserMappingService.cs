using cxOrganization.Client;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public interface IUserGroupUserMappingService
    {
        /// <summary>
        /// Map to entity from men\mbership dto
        /// </summary>
        /// <param name="membershipDto"></param>
        /// <returns></returns>
        UGMemberEntity ToUGMemberEntity(MembershipDto membershipDto, UGMemberEntity ugMemberEntity = null);
        /// <summary>
        /// Map to membership dto
        /// </summary>
        /// <param name="ugMemberEntity"></param>
        /// <returns></returns>
        MembershipDto ToMembershipDto(UGMemberEntity ugMemberEntity);
        /// <summary>
        /// Map to member dto
        /// </summary>
        /// <param name="userGroupUserEntity"></param>
        /// <param name="archetype"></param>
        /// <returns></returns>
        MemberDto ToMemberDto(UGMemberEntity ugMemberEntity, ArchetypeEnum archetype);
        /// <summary>
        /// Map to entity from member dto
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberDto"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        UGMemberEntity ToUGMemberEntity(int? userId, MemberDto memberDto, UGMemberEntity entity);
    }
}
