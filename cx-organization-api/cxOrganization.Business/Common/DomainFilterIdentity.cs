using System.Collections.Generic;
using cxOrganization.Domain.Extensions;
using cxPlatform.Client.ConexusBase;


namespace cxOrganization.Business.Common
{
    public class DomainFilterIdentity
    {
        public DomainFilterIdentity()
        {
            Ids = new List<int>();
            ExtIds = new List<string>();
            Archetypes = new List<ArchetypeEnum>();
        }

        public DomainFilterIdentity(IdentityDto identityDto)
        {
            if (identityDto != null)
            {
                if (identityDto.Id != null && identityDto.Id > 0)
                {
                    Ids = new List<int> {(int) identityDto.Id.Value};
                }

                if (!string.IsNullOrEmpty(identityDto.ExtId))
                {
                    ExtIds = new List<string> {identityDto.ExtId};
                }

                if (identityDto.Archetype != ArchetypeEnum.Unknown)
                {
                    Archetypes = new List<ArchetypeEnum> {identityDto.Archetype};
                }
                if (identityDto.CustomerId > 0)
                {
                    CustomerIds = new List<int> {identityDto.CustomerId};
                }
                OwnerId = identityDto.OwnerId;

            }
        }

        public DomainFilterIdentity(ConexusBaseDto dto) : this(dto.Identity)
        {

        }

        public List<int> Ids { get; set; }
        public List<string> ExtIds { get; set; }
        public List<ArchetypeEnum> Archetypes { get; set; }
        public List<int> CustomerIds { get; set; }
        public int OwnerId { get; set; }
        public bool CanIdentify()
        {
            return Ids.IsNotNullOrEmpty() || ExtIds.IsNotNullOrEmpty();
        }

        public static DomainFilterIdentity CreateFrom(IdentityDto identityDto)
        {
            return new DomainFilterIdentity(identityDto);
        }
        public static DomainFilterIdentity CreateFrom(ConexusBaseDto dto)
        {
            return new DomainFilterIdentity(dto);
        }
    }
}
