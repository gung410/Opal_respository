using System;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Business.Common;
using cxOrganization.Business.Validations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    [Serializable]
    public class ConnectionSourceDto
    {
        [UserGroupArchetypeIdentityValidate]
        [IdentityValidate(Required = true)]
        public IdentityDto Identity { get; set; }

        [ArchetypeIdentityValidate(ArchetypeGroup.Department, ArchetypeGroup.User)]
        [IdentityValidate]
        public IdentityDto Parent { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReferrerResource { get; set; }
        public string ReferrerToken { get; set; }
        public ArchetypeEnum? ReferrerArchetype { get; set; }

        [Required]
        public ConnectionType Type { get; set; }
        public int? MemberCount { get; set; }
        public EntityStatusDto EntityStatus { get; set; }
    }
}
