using System.Collections.Generic;
using cxPlatform.Core;
using System;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface ICustomerRepository
    /// </summary>
    public interface  ICustomerRepository : IRepository<CustomerEntity>
    {
        /// <summary>
        /// Get Customer By ExtId
        /// </summary>
        /// <param name="customerExtId"></param>
        /// <returns></returns>
        CustomerEntity GetCustomerByExtId(string customerExtId);

        /// <summary>
        /// get customers by ids and extId
        /// </summary>
        /// <param name="customerids"></param>
        /// <param name="extid"></param>
        /// <returns></returns>
        List<CustomerEntity> GetCustomers(List<int> ownerids, 
            List<int> customerids, 
            List<string> extId,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null);
    }
}
