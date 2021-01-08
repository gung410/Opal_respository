using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserGroupType.
    /// </summary>
    [Serializable]
    public class UserGroupTypeEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupTypeEntity"/> class.
        /// </summary>
        public UserGroupTypeEntity()
        {
            LT_UserGroupType = new List<LtUserGroupTypeEntity>();
        }

        /// <summary>
        /// Gets or sets the user group type Identifier.
        /// </summary>
        /// <value>The user group type Identifier.</value>
        public int UserGroupTypeId { get; set; }
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
        /// Gets or sets the type of the l t_ user group.
        /// </summary>
        /// <value>The type of the l t_ user group.</value>
        public virtual ICollection<LtUserGroupTypeEntity> LT_UserGroupType { get; set; }
    }
}
