using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class Customer.
    /// </summary>
    [Serializable]
    public class CustomerEntity
    {
        /// <summary>
        /// Gets or sets the customer Identifier.
        /// </summary>
        /// <value>The customer Identifier.</value>
        public int CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the owner Identifier.
        /// </summary>
        /// <value>The owner Identifier.</value>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public short Status { get; set; }
        /// <summary>
        /// Gets or sets the ext Identifier.
        /// </summary>
        /// <value>The ext Identifier.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the root menu Identifier.
        /// </summary>
        /// <value>
        /// The root menu Identifier.
        /// </value>
        public int? RootMenuId { get; set; }
        /// <summary>
        /// Gets or sets the HasUserIntegration.
        /// </summary>
        /// <value>The HasUserIntegration.</value>
        public bool HasUserIntegration { get; set; }
        /// <summary>
        /// Gets or sets the logo path
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// Gets or sets the Color
        /// </summary>
        public string CssVariables { get; set; }
        /// <summary>
        /// Gets or sets the Code name
        /// </summary>
        public string CodeName { get; set; }
        /// <summary>
        /// Gets or sets the favicon.
        /// </summary>
        /// <value>The favicon.</value>
        public string Favicon { get; set; }
    }
}
