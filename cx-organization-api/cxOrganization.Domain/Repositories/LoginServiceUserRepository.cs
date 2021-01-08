using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class LoginServiceUserRepository
    /// </summary>
    public class LoginServiceUserRepository : RepositoryBase<LoginServiceUserEntity>, ILoginServiceUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginServiceUserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public LoginServiceUserRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }

        public LoginServiceUserEntity GetLoginServiceUser(int userId, int loginServiceId, bool forChanging = false)
        {
            var query = forChanging ? GetAll() : GetAllAsNoTracking();
            return query.FirstOrDefault(lu => lu.UserId == userId && lu.LoginServiceId == loginServiceId);
        }
        public List<LoginServiceUserEntity> GetLoginServiceUsersOfUser(int userId, bool forChanging = false)
        {
            var query = forChanging ? GetAll() : GetAllAsNoTracking();
            return query.Where(lu => lu.UserId == userId).ToList();
        }
        public List<LoginServiceUserEntity> GetLoginServiceUsers(
            int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<string> userExtIds = null,
            List<ArchetypeEnum> userArchetypes = null,
            List<int> loginServiceIds = null,
            List<string> primaryClaimTypes = null,
            List<string> primaryClaimValues = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            List<int> siteIds = null,
            bool? includeLoginServiceHasNullSiteId = null,
            List<string> claimValues = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            bool includeUser = false,
            bool includeLoginService = false)
        {
            var userEntityStatuseIds = userEntityStatuses.IsNotNullOrEmpty() ?
                userEntityStatuses.Select(s => (int)s).ToList() :
                new List<int> { (int)EntityStatusEnum.Active, (int)EntityStatusEnum.Pending };

            var userArchetypeIds = userArchetypes == null ? null : userArchetypes.Select(a => (int)a).ToList();

            var query = LoginServiceUserQueryBuilder.InitQueryBuilder(GetAllAsNoTracking())
                .FilterWithOwnerId(ownerId)
                .FilterWithCustomerIds(customerIds)
                .FilterWithUserIds(userIds)
                .FilterWithUserExtIds(userExtIds)
                .FilterWithUserArchetypeIds(userArchetypeIds)
                .FilterWithUserEntityStatues(userEntityStatuseIds)
                .FilterWithLoginServiceIds(loginServiceIds)
                .FilterWithPrimaryClaimTypes(primaryClaimTypes)
                .FilterWithClaimValues(primaryClaimValues)
                .FilterWithSiteIds(siteIds, includeLoginServiceHasNullSiteId)
                .FilterWithClaimValues(claimValues)
                .FilterWithCreatedBefore(createdBefore)
                .FilterWithCreatedAfter(createdAfter)
                .Build();


            if (includeUser)
                query = query.Include(q => q.User);
            if (includeLoginService)
                query = query.Include(q => q.LoginService);
            return query.ToList();
        }
    }
}
