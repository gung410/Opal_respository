using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Entities;
using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// ActivityRepository
    /// </summary>
    public class PropValueRepository : RepositoryBase<PropValueEntity>, IPropValueRepository
    {
        const int MaximumRecordsReturn = 5000;
   
        /// <summary>
        /// ActivityRepository
        /// </summary>
        /// <param name="dbContext"></param>
        public PropValueRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
            
        }

        public int GetValue(int itemId, int propertyId)
        {
            var query = GetAllAsNoTracking().FirstOrDefault(p => p.PropertyId == propertyId && p.ItemId == itemId);
            return query != null ? Convert.ToInt32(query.Value) : 0;
        }

        public List<PropValueEntity> GetPropValuesByItemIds(List<int> itemIds, List<int> propertyIds)
        {
            return GetAllAsNoTracking().Where(x => propertyIds.Contains(x.PropertyId) && itemIds.Contains(x.ItemId)).ToList();
        }
        public List<PropValueEntity> GetPropValuesByItemId(int itemId, int propertyId)
        {
            return GetAllAsNoTracking().Where(x => x.PropertyId == propertyId && x.ItemId == itemId).ToList();
        }

        public PropValueEntity FindPropValueByItemIdAndPropertyId(int itemId, int propertyId)
        {
            return GetAllAsNoTracking().FirstOrDefault(p => p.PropertyId == propertyId && p.ItemId == itemId);
        }
        /// <summary>
        /// Find Prop Value Include Prop
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public PropValueEntity FindPropValueIncludeProp(int itemId, int propertyId)
        {
            return GetAllAsNoTracking().FirstOrDefault(p => p.PropertyId == propertyId && p.ItemId == itemId);
        }
    }
}