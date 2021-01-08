using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class H_D.
    /// </summary>
    [Serializable]
    public class HierarchyDepartmentEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyDepartmentEntity"/> class.
        /// </summary>
        public HierarchyDepartmentEntity()
        {
            H_Ds = new List<HierarchyDepartmentEntity>();
        }
        /// <summary>
        /// Gets or sets the hdId).
        /// </summary>
        /// <value>The hdId).</value>
        public int HDId { get; set; }
        /// <summary>
        /// Gets or sets the hierarchy Identifier.
        /// </summary>
        /// <value>The hierarchy Identifier.</value>
        public int HierarchyId { get; set; }
        /// <summary>
        /// Gets or sets the department Identifier.
        /// </summary>
        /// <value>The department Identifier.</value>
        public int DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the parent Identifier.
        /// </summary>
        /// <value>The parent Identifier.</value>
        public int? ParentId { get; set; }
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }
        /// <summary>
        /// Gets or sets the name of the path.
        /// </summary>
        /// <value>The name of the path.</value>
        public string PathName { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the deleted.
        /// </summary>
        /// <value>The deleted.</value>
        public short Deleted { get; set; }
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>The department.</value>
        public DepartmentEntity Department { get; set; }
        /// <summary>
        /// Gets or sets the h_ ds.
        /// </summary>
        /// <value>The h_ ds.</value>
        public ICollection<HierarchyDepartmentEntity> H_Ds { get; set; }
        
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public HierarchyDepartmentEntity Parent { get; set; }
       
        /// <summary>
        /// Gets or sets the hierarchy.
        /// </summary>
        /// <value>The parent.</value>
        public HierarchyEntity Hierarchy { get; internal set; }
    }
}
