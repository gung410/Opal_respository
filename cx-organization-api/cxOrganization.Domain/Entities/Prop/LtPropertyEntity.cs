using System;

namespace cxOrganization.Domain.Entities
{
    [Serializable]
    public class LtPropertyEntity
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the property Identifier.
        /// </summary>
        /// <value>The property Identifier.</value>
        public int PropertyId { get; set; }
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
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public virtual PropertyEntity Property { get; set; }
    }
}
