using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtUserType.
    /// </summary>
    [Serializable]
    public class LtUserTypeEntity
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the user type Identifier.
        /// </summary>
        /// <value>The user type Identifier.</value>
        public int UserTypeId { get; set; }
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
        /// Gets or sets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        public virtual UserTypeEntity UserType { get; set; }
    }
}