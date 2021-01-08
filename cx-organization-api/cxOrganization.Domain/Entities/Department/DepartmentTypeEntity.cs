using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class DepartmentType.
    /// </summary>
    [Serializable]
    public class DepartmentTypeEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentTypeEntity"/> class.
        /// </summary>
        public DepartmentTypeEntity()
        {
            LT_DepartmentType = new List<LtDepartmentTypeEntity>();
        }

        /// <summary>
        /// Gets or sets the department type Identifier.
        /// </summary>
        /// <value>The department type Identifier.</value>
        public int DepartmentTypeId { get; set; }
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
        /// Gets or sets the type of the l t_ department.
        /// </summary>
        /// <value>The type of the l t_ department.</value>
        public ICollection<LtDepartmentTypeEntity> LT_DepartmentType { get; set; }
        public ICollection<DTUGEntity> DT_UGs { get; set; }
        public ICollection<DTDEntity> DT_Ds { get; set; }
    }
}
