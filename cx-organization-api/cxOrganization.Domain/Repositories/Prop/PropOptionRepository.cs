using System.Linq;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    public class PropOptionRepository : RepositoryBase<PropOptionEntity>, IPropOptionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropOptionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public PropOptionRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the property option by identifier.
        /// </summary>
        /// <param name="propOptionId">The property option identifier.</param>
        /// <returns>PropOption.</returns>
        public PropOptionEntity GetPropOptionById(int propOptionId)
        {
            return GetAll().Include(lt => lt.LtPropOptions).SingleOrDefault(t => t.PropOptionId == propOptionId);
        }
    }
}
