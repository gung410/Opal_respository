using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using System.Collections.Generic;

namespace cxOrganization.Domain.Repositories
{
    public interface IUserGroupTypeRepository : IRepository<UserGroupTypeEntity>
    {
        List<UserGroupTypeEntity> GetUserGroupTypes(int ownerId, List<int> userGroupIds = null, List<ArchetypeEnum> archetypeIds = null, List<int> userGroupTypeIds = null, List<string> extIds = null);
    }
}
