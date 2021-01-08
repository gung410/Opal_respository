using System;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LT_OwnerColor.
    /// </summary>
    [Serializable]
    public class LtOwnerColorEntity
    {
        /// <summary>
        /// Gets or sets the owner color identifier.
        /// </summary>
        /// <value>The owner color identifier.</value>
        public int OwnerColorId { get; set; }
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the color of the owner.
        /// </summary>
        /// <value>The color of the owner.</value>
        public virtual OwnerColorEntity OwnerColor { get; set; }
    }
}
