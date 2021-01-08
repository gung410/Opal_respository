using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LT_LoginService.
    /// </summary>
     [Serializable]
	public class LtLoginService
	{
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
		public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the login service Identifier.
        /// </summary>
        /// <value>The login service Identifier.</value>
		public int LoginServiceId { get; set; }
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
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
		public string ToolTip { get; set; }
        /// <summary>
        /// Gets or sets the login service.
        /// </summary>
        /// <value>The login service.</value>
		public LoginServiceEntity LoginServiceEntity { get; set; }
	}
}

