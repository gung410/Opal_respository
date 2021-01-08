using System.Collections.Generic;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface ILoginServiceRepository
    /// </summary>
    public interface ILoginServiceRepository : IRepository<LoginServiceEntity>
    {
        List<LoginServiceEntity> GetLoginServices(
            List<int> loginServiceIds = null,
            List<string> issuers = null,
            List<string> primaryClaimTypes = null,
            List<string> secondaryClaimTypes = null,
            List<int> siteIds = null);
    }
}
