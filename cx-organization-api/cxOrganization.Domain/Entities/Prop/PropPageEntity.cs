using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PropPage.
    /// </summary>
    [Serializable]
    public class PropPageEntity
    {
        // <summary>
        /// Initializes a new instance of the <see cref="PropPage"/> class.
        /// </summary>
        public PropPageEntity()
        {
            Properties = new List<PropertyEntity>();
        }
        /// <summary>
        /// Gets or sets the property page Identifier.
        /// </summary>
        /// <value>The property page Identifier.</value>
        public int PropPageId { get; set; }
        /// <summary>
        /// Gets or sets the table type Identifier.
        /// </summary>
        /// <value>The table type Identifier.</value>
        public short TableTypeId { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public short Type { get; set; }
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
        /// Gets or sets the props.
        /// </summary>
        /// <value>The props.</value>
        public virtual ICollection<PropertyEntity> Properties { get; set; }
    }
}
