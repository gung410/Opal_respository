using cxOrganization.Domain.Entities;

using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    public class UTURepository : RepositoryBase<UTUEntity>, IUTURepository
    {
        public UTURepository(OrganizationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
