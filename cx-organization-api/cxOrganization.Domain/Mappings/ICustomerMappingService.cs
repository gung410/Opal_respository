using cxOrganization.Client.Customers;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public interface ICustomerMappingService
    {
        CustomerDto ToCustomerDto(CustomerEntity customer);
        CustomerEntity ToCustomerEntity(CustomerDto customerDto, CustomerEntity entity);
        IdentityStatusDto ToIdentityStatusDto(CustomerEntity customer);
    }
}
