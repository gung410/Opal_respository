using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtDepartmentGroup.
    /// </summary>
    [Serializable]
    public class LtDepartmentGroup
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the department group Identifier.
        /// </summary>
        /// <value>The department group Identifier.</value>
        public int DepartmentGroupId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the department group.
        /// </summary>
        /// <value>The department group.</value>
        public virtual DepartmentGroupEntity DepartmentGroup { get; set; }
    }
}