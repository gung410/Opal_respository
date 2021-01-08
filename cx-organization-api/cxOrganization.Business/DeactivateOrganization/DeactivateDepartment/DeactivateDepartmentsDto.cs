using System.Collections.Generic;
using cxOrganization.Business.Common;
using cxOrganization.Business.Validations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateDepartment
{
    public class DeactivateDepartmentsDto
    {
        /// <summary>
        /// A list of department identities that will be deactivated
        /// </summary>
        [IdentityValidate(Required = false)]
        [DepartmentArchetypeIdentityValidate]
        public List<IdentityDto> Identities { get; set; }   

        /// <summary>
        /// Set true to deactivate department even that department or children department containing user
        /// </summary>
        public bool? DeactivateIfContainingUser { get; set; }

        public List<EntityStatusEnum> UserEntityStatusesForChecking { get; set; }
        /// <summary>
        /// Set true to deactivate department even that containing child department
        /// </summary>
        public bool? DeactivateIfContainingChildDepartment { get; set; }


        /// <summary>
        /// A identity of user who execute deactivating. 
        /// </summary>
        [IdentityValidate(Required = false)]
        [UserArchetypeIdentityValidate]
        public IdentityDto UpdatedByIdentity { get; set; }
    }
}
