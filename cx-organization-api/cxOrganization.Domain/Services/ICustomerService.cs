using System;
using System.Collections.Generic;
using cxOrganization.Client.Customers;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Services
{
    public interface ICustomerService
    {
        CustomerDto UpdateCustomer(CustomerDto customer);
        CustomerDto InsertCustomer(CustomerDto customer);
        List<CustomerDto> GetCustomers(List<int> customerids = null,
            List<int> ownerids=null, 
            List<string> extids = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null);
        CustomerEntity GetCustomerById(int customerId);
    }
}
