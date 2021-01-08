using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtPropOption.
    /// </summary>
    [Serializable]
    public class LtPropOptionEntity
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the property option Identifier.
        /// </summary>
        /// <value>The property option Identifier.</value>
        public int PropOptionId { get; set; }
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
        /// Gets or sets the property option.
        /// </summary>
        /// <value>The property option.</value>
        public virtual PropOptionEntity PropOption { get; set; }
    }
}
