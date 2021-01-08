using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class U_D.
    /// </summary>
    [Serializable]
    public class UserDepartmentEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDepartmentEntity"/> class.
        /// </summary>
        public UserDepartmentEntity()
        {
            //UserTypes = new List<UserTypeEntity>();
        }

        /// <summary>
        /// Gets or sets the u_ dId).
        /// </summary>
        /// <value>The u_ dId).</value>
        public int U_DId { get; set; }
        /// <summary>
        /// Gets or sets the department Identifier.
        /// </summary>
        /// <value>The department Identifier.</value>
        public int DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the user Identifier.
        /// </summary>
        /// <value>The user Identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UserDepartmentEntity"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public virtual UserEntity User { get; set; }
        /// <summary>
        /// Gets or sets the user types.
        /// </summary>
        /// <value>The user types.</value>
        //public ICollection<UserTypeEntity> UserTypes { get; set; }
        public ICollection<UDUTEntity> U_D_UTs { get; set; }
    }
}