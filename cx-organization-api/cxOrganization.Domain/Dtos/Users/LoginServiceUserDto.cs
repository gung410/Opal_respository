using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Dtos.Users
{
    /// <summary>
    /// The definition of claim value for the user in a specify login service
    /// </summary>
    public class LoginServiceUserDto : IConexusBaseDto
    {
        /// <summary>
        /// Identity of user. This is used to identify user base on id or extId or both with specify archetype, in given customer and owner.
        /// If customer, owner is not set or set with value less than or equal zero, they will be set with current one.
        /// </summary>
        [Required]
        public IdentityDto UserIdentity { get; set; }

        /// <summary>
        /// Identity of login service. This is used to identify login service by id or extId, or both. 
        /// If siteId has value, login service is identify in this siteId only.
        /// In this, ExtId will be mapped with PrimaryClaimValue of login service entity,  since it doesn't have field ExtId yet
        /// </summary>


        public LoginServiceIdentityDto LoginServiceIdentity { get; set; }

        /// <summary>
        /// Primary claim value of user in login service
        /// </summary>
        public string ClaimValue { get; set; }

        /// <summary>
        /// Created date-time when claim value is added for user. If is not set value,  it will be set with current date-time 
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// The list of entity status that user is allowed to retrieve 
        /// </summary>
        public List<EntityStatusEnum> AcceptedUserEntityStatuses { get; set; }

    }
}
