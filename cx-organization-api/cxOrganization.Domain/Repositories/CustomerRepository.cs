using System;
using System.Collections.Generic;
using System.Linq;
using cxPlatform.Core;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Class CustomerRepository
    /// </summary>
    public class CustomerRepository : RepositoryBase<CustomerEntity>,ICustomerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>
        public CustomerRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
            
        }

        /// <summary>
        /// Get Customer By ExtId
        /// </summary>
        /// <param name="customerExtId"></param>
        /// <returns></returns>
        public CustomerEntity GetCustomerByExtId(string customerExtId)
        {
            return GetAll().FirstOrDefault(x => x.ExtId == customerExtId);
        }

        public List<CustomerEntity> GetCustomers(List<int> ownerids,
            List<int> customerids, 
            List<string> extids, 
            DateTime? createdBefore = null,
            DateTime? createdAfter = null)
        {
            var query = GetAllAsNoTracking();
            if (ownerids != null && ownerids.Any())
            {
                query = query.Where(t=>ownerids.Contains(t.OwnerId));
            }
            if (customerids != null && customerids.Any())
            {
                query = query.Where(t => customerids.Contains(t.CustomerId));
            }
            if (extids != null && extids.Any())
            {
                query = query.Where(t =>extids.Contains(t.ExtId));
            }
            if (createdBefore.HasValue)
            {
                query = query.Where(p => p.Created <= createdBefore);
            }
            if (createdAfter.HasValue)
            {
                query = query.Where(p => p.Created >= createdAfter);
            }
            return query.ToList();
        }
    }
}
