using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxOrganization.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IUserTypeRepository
    /// </summary>
    public interface IUserTypeRepository : IRepository<UserTypeEntity>
    {
        List<UserTypeEntity> GetUserTypes(int ownerId = 0,
           List<int> userIds = null,
           List<ArchetypeEnum> archetypeIds = null,
           List<int> userTypeIds = null,
           List<string> extIds = null,
           bool includeLocalizedData = false,
           List<int> parentIds = null);
        UserTypeEntity GetUserTypeByExtId(string extId, int? archetypeId = null);
        List<UserTypeEntity> GetAllUserTypesInCache();
        ILookup<string, UserTypeEntity> GetAllUserTypesLookupByExtIdInCache();
        Task<List<UserTypeEntity>> GetAllUserTypesInCacheAsync();

    }
}
