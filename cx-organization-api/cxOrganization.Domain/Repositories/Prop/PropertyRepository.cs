using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    public class PropertyRepository : RepositoryBase<PropertyEntity>, IPropertyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public PropertyRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }
        
        public List<PropertyEntity> GetProperties()
        {
            return GetAllAsNoTracking().Include(t => t.LtProperties).ToList();
        }
    }
}
