using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LevelLimit.
    /// </summary>
    [Serializable]
    public class LevelLimitEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelLimit"/> class.
        /// </summary>
        public LevelLimitEntity()
        {
            LT_LevelLimits = new List<LtLevelLimitEntity>();
        }
        /// <summary>
        /// Gets or sets the alternative identifier.
        /// </summary>
        /// <value>The alternative identifier.</value>
        public int? AlternativeId { get; set; }
        /// <summary>
        /// Gets or sets the level limit identifier.
        /// </summary>
        /// <value>The level limit identifier.</value>
        public int LevelLimitId { get; set; }
        /// <summary>
        /// Gets or sets the level group identifier.
        /// </summary>
        /// <value>The level group identifier.</value>
        public int? LevelGroupId { get; set; }
        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        /// <value>The category identifier.</value>
        public int? CategoryId { get; set; }
        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        /// <value>The question identifier.</value>
        public int? QuestionId { get; set; }
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public double MinValue { get; set; }
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public double MaxValue { get; set; }
        /// <summary>
        /// Gets or sets the sigchange.
        /// </summary>
        /// <value>The sigchange.</value>
        public double Sigchange { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [negative trend].
        /// </summary>
        /// <value><c>true</c> if [negative trend]; otherwise, <c>false</c>.</value>
        public bool NegativeTrend { get; set; }
        /// <summary>
        /// Gets or sets the owner color identifier.
        /// </summary>
        /// <value>The owner color identifier.</value>
        public int OwnerColorId { get; set; }
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>The item identifier.</value>
        public int? ItemId { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the ext identifier.
        /// </summary>
        /// <value>The ext identifier.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets the l t_ level limit.
        /// </summary>
        /// <value>The l t_ level limit.</value>
        public virtual ICollection<LtLevelLimitEntity> LT_LevelLimits { get; set; }

        /// <summary>
        /// Gets or sets the matching type.
        /// </summary>
        /// <value>The matching type.</value>
        public int MatchingType { get; set; }

        /// <summary>
        /// LevelGroup
        /// </summary>
        public virtual LevelGroupEntity LevelGroup { get; set; }
        /// <summary>
        /// LevelGroup
        /// </summary>
        public virtual OwnerColorEntity OwnerColor { get; set; }
    }
}
