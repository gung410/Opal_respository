using System;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LT_LevelLimit.
    /// </summary>
    [Serializable]
    public class LtLevelLimitEntity
    {
        /// <summary>
        /// Gets or sets the level limit identifier.
        /// </summary>
        /// <value>The level limit identifier.</value>
        public int LevelLimitId { get; set; }
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
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the level limit.
        /// </summary>
        /// <value>The level limit.</value>
        public virtual LevelLimitEntity LevelLimit { get; set; }
    }
}
