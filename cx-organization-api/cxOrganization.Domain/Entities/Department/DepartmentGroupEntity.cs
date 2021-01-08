using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class DepartmentGroup.
    /// </summary>
    [Serializable]
    public class DepartmentGroupEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentGroupEntity"/> class.
        /// </summary>
        public DepartmentGroupEntity()
        {
            LT_DepartmentGroup = new List<LtDepartmentGroup>();
            //Departments = new List<DepartmentEntity>();
        }

        /// <summary>
        /// Gets or sets the department group Identifier.
        /// </summary>
        /// <value>The department group Identifier.</value>
        public int DepartmentGroupId { get; set; }
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
        /// Gets or sets the l t_ department group.
        /// </summary>
        /// <value>The l t_ department group.</value>
        public virtual ICollection<LtDepartmentGroup> LT_DepartmentGroup { get; set; }
        /// <summary>
        /// Gets or sets the departments.
        /// </summary>
        /// <value>The departments.</value>
        //public virtual ICollection<DepartmentEntity> Departments { get; set; }
        public ICollection<DGDEntity> DG_Ds { get; set; }
    }
}