using System.Collections.Generic;
using System.ComponentModel;
using cxOrganization.Business.Validations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.MoveOrganization.MoveDepartment
{
    /// <summary>
    /// A definition information to move departments
    /// </summary>
    public class MoveDepartmentsDto
    {
        public MoveDepartmentsDto()
        {
            ForceUserLoginAgain = true;
        }

        /// <summary>
        /// List identities of department that will be moved into target parent
        /// </summary>
        /// 
        [IdentityValidate(Required = true)]
        [DepartmentArchetypeIdentityValidate]
        public List<IdentityDto> Identities { get; set; }



        /// <summary>
        /// An identity of department that another departments will be moved into
        /// </summary>

        [IdentityValidate(Required = true)]
        [DepartmentArchetypeIdentityValidate]
        public IdentityDto TargetParent { get; set; }

        /// <summary>
        /// A identity of user who execute moving.
        /// </summary>
        [IdentityValidate(Required = false)]
        [UserArchetypeIdentityValidate]
        public IdentityDto UpdatedByIdentity { get; set; }

        /// <summary>
        /// Set true to force users who are in source department to login again. Default value is true.
        /// </summary>
        [DefaultValue(true)]
        public bool ForceUserLoginAgain { get; set; }

    }


}