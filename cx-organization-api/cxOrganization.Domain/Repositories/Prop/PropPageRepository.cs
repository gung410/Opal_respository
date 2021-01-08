using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class PropPageRepository.
    /// </summary>
    public class PropPageRepository : RepositoryBase<PropPageEntity>, IPropPageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropPageRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public PropPageRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the property pages by table type identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>List{PropPage}.</returns>
        public List<PropPageEntity> GetPropPagesByTableTypeId(int Id)
        {
            return GetAll()
                .Include(t => t.Properties)
                .Where(t => t.TableTypeId == Id)
                .OrderBy(t => t.No).ToList();
        }
    }
}
