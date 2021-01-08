using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class LoginServiceRepository.
    /// </summary>
    public class LoginServiceRepository : RepositoryBase<LoginServiceEntity>, ILoginServiceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginServiceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public LoginServiceRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }

        public List<LoginServiceEntity> GetLoginServices(
            List<int> loginServiceIds = null,
            List<string> issuers = null,
            List<string> primaryClaimTypes = null,
            List<string> secondaryClaimTypes = null,
            List<int> siteIds = null)
        {
            var query = LoginServiceQueryBuilder.InitQueryBuilder(GetAllAsNoTracking())
                .FilterWithIds(loginServiceIds)
                .FilterWithIssuers(issuers)
                .FilterWithPrimaryClaimTypes(primaryClaimTypes)
                .FilterWithSecondaryClaimTypes(secondaryClaimTypes)
                .FilterWithSiteIds(siteIds)
                .Build();
            return query.ToList();
        }


    }
}
