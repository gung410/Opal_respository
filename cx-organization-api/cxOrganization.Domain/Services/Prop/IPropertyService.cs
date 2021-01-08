using System.Collections.Generic;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IPropertyService
    {
        Dictionary<int, List<EntityKeyValueDto>> GetDynamicProperties(List<int> itemIds, TableTypes tableType);
        List<EntityKeyValueDto> GetDynamicProperties(int itemId, TableTypes tableType);
        PropValueEntity FindPropValueIncludeProp(int itemId, int propertyId);
    }
}
