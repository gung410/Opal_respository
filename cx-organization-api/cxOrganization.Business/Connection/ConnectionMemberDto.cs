using System;
using System.Collections.Generic;
using cxOrganization.Business.Validations;
using cxOrganization.Client.UserGroups;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    [Serializable]
    [ConnectionMemberValidate]
    public class ConnectionMemberDto
    {
        public IdentityDto Identity { get; set; }

        [UserArchetypeIdentityValidate]
        [IdentityValidate]
        public IdentityDto UserIdentity { get; set; }
        public DateTime? Created { get; set; }
        public int CreatedBy { get; set; }
        public MemberRoleEnum? MemberRoleId { get; set; }
        public DateTime? validFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public int? PeriodId { get; set; }
        public List<EntityKeyValueDto> DynamicAttributes { get; set; }
        public EntityStatusDto EntityStatus { get; set; }
        public string ReferrerResource { get; set; }
        public string ReferrerToken { get; set; }
        public ArchetypeEnum? ReferrerArchetype { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? MobileCountryCode { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public short? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    
        /// <summary>
        /// Only for getting.
        /// </summary>
        public ConnectionSourceDto Source { get; set; }

        public bool IsUserMember()
        {
            return UserIdentity != null;
        }
        public bool IsNonuserMember()
        {
            return UserIdentity == null;
        }
    }
}
