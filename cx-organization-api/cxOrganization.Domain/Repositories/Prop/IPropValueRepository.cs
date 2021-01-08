using cxPlatform.Core;
using System.Collections.Generic;
using cxOrganization.Domain.Entities;
using System;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// IEventRepository
    /// </summary>
    public interface IPropValueRepository : IRepository<PropValueEntity>
    {
        int GetValue(int departmentId, int ropPeriodId);
        List<PropValueEntity> GetPropValuesByItemIds(List<int> itemIds, List<int> propertyIds);
        List<PropValueEntity> GetPropValuesByItemId(int itemId, int propertyId);
        PropValueEntity FindPropValueByItemIdAndPropertyId(int itemId, int propertyId);
        PropValueEntity FindPropValueIncludeProp(int itemId, int propertyId);
    }
}
