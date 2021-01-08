using System.Collections.Generic;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public class UserGroupTypeService : IUserGroupTypeService
    {
        private readonly IUserGroupTypeRepository _userGroupTypeRepository;
        public UserGroupTypeService(IUserGroupTypeRepository userGroupTypeRepository)
        {
            _userGroupTypeRepository = userGroupTypeRepository;
        }
        public List<IdentityStatusDto> GetUserGroupTypes(int ownerId, 
            List<int> userGroupIds = null, 
            List<ArchetypeEnum> archetypeIds = null, 
            List<int> userGroupTypeIds = null, 
            List<string> extIds = null)
        {
            var userGroupTypeEntities = _userGroupTypeRepository.GetUserGroupTypes(ownerId: ownerId,
               userGroupIds: userGroupIds,
               archetypeIds: archetypeIds,
               userGroupTypeIds: userGroupTypeIds,
               extIds: extIds);
            List<IdentityStatusDto> result = new List<IdentityStatusDto>();
            foreach (var userGroupTypeEntity in userGroupTypeEntities)
            {
                result.Add(ToIdentityStatusDto(userGroupTypeEntity));
            }
            return result;
        }
        private IdentityStatusDto ToIdentityStatusDto(UserGroupTypeEntity userGroupTypeEntity)
        {
            return new IdentityStatusDto
            {
                Identity = new IdentityDto
                {
                    Id = userGroupTypeEntity.UserGroupTypeId,
                    Archetype = ArchetypeEnum.Unknown,
                    ExtId = userGroupTypeEntity.ExtId,
                    OwnerId = userGroupTypeEntity.OwnerId
                },
                EntityStatus = new EntityStatusDto()

            };
        }
    }
}
