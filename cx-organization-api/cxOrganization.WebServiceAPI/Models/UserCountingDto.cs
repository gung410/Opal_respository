using cxOrganization.Business.Extensions;
using cxOrganization.Domain.Dtos.Users;
using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;
using System.Linq;

namespace cxOrganization.WebServiceAPI.Models
{
    public class UserCountingDto : UserCountByUserTypeDto
    {
        public ArchetypeEnum Archetype { get { return (ArchetypeEnum)this.ArchetypeId; } }

        public static List<UserCountingDto> CreateListFrom(List<UserCountByUserTypeDto> userCountByUserTypeDtos)
        {
            if (userCountByUserTypeDtos.IsNullOrEmpty()) return new List<UserCountingDto>();

            return userCountByUserTypeDtos.Select(userCount => new UserCountingDto()
            {
                UserTypeId = userCount.UserTypeId,
                UserTypeExtId = userCount.UserTypeExtId,
                UserTypeName = userCount.UserTypeName,
                Count = userCount.Count,
                ArchetypeId = userCount.ArchetypeId
            }).ToList();
        }
    }
}
