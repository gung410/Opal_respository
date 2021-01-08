using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Validators
{
    public interface ICustomerValidator
    {
        CustomerEntity Validate(ConexusBaseDto dto);
    }
}
