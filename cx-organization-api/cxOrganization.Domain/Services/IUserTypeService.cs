using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IUserTypeService
    {
        /// <summary>
        /// Get User Types 
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="userTypeIds"></param>
        /// <param name="includeLocalizedData"></param>
        /// <returns></returns>
        List<UserTypeDto> GetUserTypes(int ownerId, List<int> userIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> extIds = null,
            bool includeLocalizedData = false,
            string langCode = "en-US",
            List<int> parentIds = null);
        UserTypeEntity GetUserTypeByExtId(string extId, int? archeTypeId = null);
        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <param name="extIds"></param>
        /// <param name="archetypeEnums"></param>
        /// <param name="userTypeIds"></param>
        /// <returns></returns>
        List<UserTypeDto> GetUserRoles(List<string> extIds = null,
            List<ArchetypeEnum> archetypeEnums = null,
            List<int> userTypeIds = null,
            List<int> userIds = null);
        /// <summary>
        /// Add User type user
        /// </summary>
        /// <param name="userTypeId"></param>
        /// <param name="userMemberDto"></param>
        /// <param name="checkingUserArchetypeIds"></param>
        /// <returns></returns>
        MemberDto AddUserTypeUser(int userTypeId, MemberDto userMemberDto, List<int> checkingUserArchetypeIds = null);
        /// <summary>
        /// Remove User type user
        /// </summary>
        /// <param name="userTypeId"></param>
        /// <param name="userMemberDto"></param>
        /// <param name="checkingUserArchetypeIds"></param>
        /// <returns></returns>
        MemberDto RemoveUserTypeUser(int userTypeId, MemberDto userMemberDto, List<int> checkingUserArchetypeIds = null);
        MemberDto UpdateOrInsertUserTypeUser(int userId,
            MemberDto levelMemberDto,
            bool isUnique = false);
        MemberDto DeleteUserTypeUser(int userId,
            MemberDto levelMemberDto);
    }
}
