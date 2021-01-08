using System;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LT_LevelGroup.
    /// </summary>
    [Serializable]
    public class LtLevelGroupEntity
    {
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the level group identifier.
        /// </summary>
        /// <value>The level group identifier.</value>
        public int LevelGroupId { get; set; }
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
        /// Gets or sets the level group.
        /// </summary>
        /// <value>The level group.</value>
        public virtual LevelGroupEntity LevelGroup { get; set; }
    }
}
