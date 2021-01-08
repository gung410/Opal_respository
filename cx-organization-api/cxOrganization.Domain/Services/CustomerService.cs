using System;
using System.Collections.Generic;
using cxOrganization.Client.Customers;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Validators;

namespace cxOrganization.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerMappingService _customerMappingService;
        private readonly ICustomerValidator _customerValidator;
        private readonly OrganizationDbContext _organizationDbContext;

        public CustomerService(ICustomerRepository customerRepository,
            ICustomerMappingService customerMappingService,
            ICustomerValidator customerValidator,
            OrganizationDbContext organizationDbContext)
        {
            _customerRepository = customerRepository;
            _customerMappingService = customerMappingService;
            _customerValidator = customerValidator;
            _organizationDbContext = organizationDbContext;
        }

        public CustomerDto UpdateCustomer(CustomerDto customer)
        {
            //Do the validation
            var entity = _customerValidator.Validate(customer);
            var customerEntity = _customerMappingService.ToCustomerEntity(customer, entity);

            _customerRepository.Update(customerEntity);
            _organizationDbContext.SaveChanges();
            return _customerMappingService.ToCustomerDto(customerEntity);
        }

        public CustomerDto InsertCustomer(CustomerDto customer)
        {
            //Do the validation
            var entity = _customerValidator.Validate(customer);
            var customerEntity = _customerMappingService.ToCustomerEntity(customer, entity);

            _customerRepository.Insert(customerEntity);
            _organizationDbContext.SaveChanges();
            return _customerMappingService.ToCustomerDto(customerEntity);
        }

        public List<CustomerDto> GetCustomers(List<int> customerids = null,
            List<int> ownerids = null,
            List<string> extids = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null)
        {
            var customerEntities = _customerRepository.GetCustomers(ownerids, customerids, extids, createdBefore, createdAfter);
            List<CustomerDto> result = new List<CustomerDto>();
            foreach (var customer in customerEntities)
            {
                var customerDto = _customerMappingService.ToCustomerDto(customer);
                if (customerDto != null)
                {
                    result.Add(customerDto);
                }
            }
            return result;
        }
        public CustomerEntity GetCustomerById(int customerId)
        {
            return _customerRepository.GetById(customerId);
        }
    }
}
