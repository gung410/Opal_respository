using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserType.
    /// </summary>
    [Serializable]
    public class UserTypeEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTypeEntity"/> class.
        /// </summary>
        public UserTypeEntity()
        {
            LT_UserType = new List<LtUserTypeEntity>();
         
        }

        /// <summary>
        /// Gets or sets the user type Identifier.
        /// </summary>
        /// <value>The user type Identifier.</value>
        public int UserTypeId { get; set; }
        /// <summary>
        /// Gets or sets the owner Identifier.
        /// </summary>
        /// <value>The owner Identifier.</value>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets the ext Identifier.
        /// </summary>
        /// <value>The ext Identifier.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the archetype Identifier.
        /// </summary>
        /// <value>The created.</value>
        public int? ArchetypeId { get; set; }
        /// <summary>
        /// Gets or sets the parent Identifier.
        /// </summary>
        /// <value>The ParentId).</value>
        public int? ParentId { get; set; }
        /// <summary>
        /// Gets or sets the type of the l t_ user.
        /// </summary>
        /// <value>The type of the l t_ user.</value>
        public ICollection<LtUserTypeEntity> LT_UserType { get; set; }
        public ICollection<UDUTEntity> U_D_UTs { get; set; }
        public ICollection<UTUEntity> UT_Us { get; set; }
    }
}
