using System;
using System.Collections.Generic;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface ILoginServiceUserRepository
    /// </summary>
    public interface ILoginServiceUserRepository : IRepository<LoginServiceUserEntity>
    {
        LoginServiceUserEntity GetLoginServiceUser(int userId, int loginServiceId, bool forChanging = false);
        List<LoginServiceUserEntity> GetLoginServiceUsersOfUser(int userId, bool forChanging = false);
        List<LoginServiceUserEntity> GetLoginServiceUsers(
            int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<int> loginServiceIds = null,
            List<string> primaryClaimTypes = null, List<string> primaryClaimValues = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            List<int> siteIds = null,
            bool? includeLoginServiceHasNullSiteId = null,
            List<string> claimValues = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            bool includeUser = false,
            bool includeLoginService = false);
    }
}
