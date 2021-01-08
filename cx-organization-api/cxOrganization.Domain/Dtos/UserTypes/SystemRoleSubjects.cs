using cxOrganization.Client.UserTypes;
using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;

namespace cxOrganization.Domain.Dtos.UserTypes
{
    public class SystemRoleSubjects
    {
        public int Id { get; set; }
        public string ExtId { get; set; }
        public SystemRolePermissionSubject SystemRolePermissionSubject { get; set; }
        public bool? IsDefaultSystemRole { get; set; }
        public List<LocalizedDataDto> LocalizedData { get; set; }

        public UserTypeDto convertToUserType(int ownerId, int customerId)
        {
            var userType = new UserTypeDto();
            userType.LocalizedData = this.LocalizedData;
            userType.Identity = new IdentityDto()
            {
                Archetype = ArchetypeEnum.SystemRole,
                ExtId = this.ExtId,
                OwnerId = ownerId,
                CustomerId = customerId,
                Id = this.Id
            };

            return userType;
        }
    }

    public class SystemRolePermissionSubject
    {

    }
}
