using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IUserGroupTypeService
    {
        List<IdentityStatusDto> GetUserGroupTypes(int ownerId, List<int> userGroupIds = null, List<ArchetypeEnum> archetypeIds = null, List<int> userGroupTypeIds = null, List<string> extIds = null);
    }
}
