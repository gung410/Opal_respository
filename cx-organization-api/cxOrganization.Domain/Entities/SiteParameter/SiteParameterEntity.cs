using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class SiteParameter.
    /// </summary>
     [Serializable]
	public class SiteParameterEntity
	{
        /// <summary>
        /// Gets or sets the site parameter Identifier.
        /// </summary>
        /// <value>The site parameter Identifier.</value>
		public int SiteParameterId { get; set; }
        /// <summary>
        /// Gets or sets the site Identifier.
        /// </summary>
        /// <value>The site Identifier.</value>
		public int SiteId { get; set; }
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
		public string Key { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
		public string Value { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
		public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The group name.</value>
        public string GroupName { get; set; }
	}
}

