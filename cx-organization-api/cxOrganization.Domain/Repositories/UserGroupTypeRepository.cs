using cxPlatform.Core;
using System.Collections.Generic;
using System.Linq;
using cxPlatform.Client.ConexusBase;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Repositories
{
    public class UserGroupTypeRepository : RepositoryBase<UserGroupTypeEntity>, IUserGroupTypeRepository
    {
        public UserGroupTypeRepository(OrganizationDbContext dbContext)
            : base(dbContext)
        {
        }
      
        public List<UserGroupTypeEntity> GetUserGroupTypes(int ownerId,
            List<int> userGroupIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userGroupTypeIds = null,
            List<string> extIds = null)
        {
            var query = GetAllAsNoTracking();
            if (ownerId > 0)
            {
                query = query.Where(t => t.OwnerId == ownerId);
            }
            if (userGroupTypeIds != null && userGroupTypeIds.Any())
            {
                query = query.Where(t => userGroupTypeIds.Contains(t.UserGroupTypeId));
            }
            if (extIds != null && extIds.Any())
            {
                query = query.Where(t => extIds.Contains(t.ExtId));
            }       
            return query.ToList();
        }
    }
}
