using System;

namespace cxOrganization.Client.Departments
{
    /// <summary>
    /// Class DepartmentDto.
    /// </summary>
    [Serializable]
    public class DepartmentDto
    {
        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        /// <value>The department identifier.</value>
        public int DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the department name
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Gets or sets parentId of department
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// Gets or sets the hd identifier.
        /// </summary>
        /// <value>
        /// The hd identifier.
        /// </value>
        public int HdId { get; set; }
        
    }
}
