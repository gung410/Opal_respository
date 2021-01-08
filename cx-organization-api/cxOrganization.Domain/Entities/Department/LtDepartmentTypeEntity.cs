using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtDepartmentType.
    /// </summary>
    [Serializable]
    public class LtDepartmentTypeEntity
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the department type Identifier.
        /// </summary>
        /// <value>The department type Identifier.</value>
        public int DepartmentTypeId { get; set; }
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
        /// Gets or sets the type of the department.
        /// </summary>
        /// <value>The type of the department.</value>
        public virtual DepartmentTypeEntity DepartmentType { get; set; }
    }
}