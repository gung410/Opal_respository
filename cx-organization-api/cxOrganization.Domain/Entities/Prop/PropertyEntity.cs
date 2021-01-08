using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class Prop.
    /// </summary>
    [Serializable]
    public class PropertyEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Prop"/> class.
        /// </summary>
        public PropertyEntity()
        {
        }

        /// <summary>
        /// Gets or sets the property Identifier.
        /// </summary>
        /// <value>The property Identifier.</value>
        public int PropertyId { get; set; }
        /// <summary>
        /// Gets or sets the property page Identifier.
        /// </summary>
        /// <value>The property page Identifier.</value>
        public int PropPageId { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public short Type { get; set; }
        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public short ValueType { get; set; }
        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [multi value].
        /// </summary>
        /// <value><c>true</c> if [multi value]; otherwise, <c>false</c>.</value>
        public bool MultiValue { get; set; }
        /// <summary>
        /// Gets or sets the l t_ property.
        /// </summary>
        /// <value>The l t_ property.</value>
        public virtual ICollection<LtPropertyEntity> LtProperties { get; set; }
        /// <summary>
        /// Gets or sets the property page.
        /// </summary>
        /// <value>The property page.</value>
        public virtual PropPageEntity PropPage { get; set; }
    }
}
