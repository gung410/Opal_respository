using cxOrganization.Domain.Entities;
using System.Collections.Generic;

namespace cxOrganization.Domain.Repositories
{
    public interface IPropertyRepository
    {
        List<PropertyEntity> GetProperties();
    }
}
