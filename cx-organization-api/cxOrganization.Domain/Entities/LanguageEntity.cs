using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class Language.
    /// </summary>
    [Serializable]
    public class LanguageEntity
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode { get; set; }
        /// <summary>
        /// Gets or sets the dir.
        /// </summary>
        /// <value>The dir.</value>
        public string Dir { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name of the native.
        /// </summary>
        /// <value>The name of the native.</value>
        public string NativeName { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
    }
}
