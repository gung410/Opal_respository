using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LT_UserGroupType.
    /// </summary>
    [Serializable]
    public class LtUserGroupTypeEntity
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the user group type Identifier.
        /// </summary>
        /// <value>The user group type Identifier.</value>
        public int UserGroupTypeId { get; set; }
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
        /// Gets or sets the type of the user group.
        /// </summary>
        /// <value>The type of the user group.</value>
        public virtual UserGroupTypeEntity UserGroupType { get; set; }
    }
}
