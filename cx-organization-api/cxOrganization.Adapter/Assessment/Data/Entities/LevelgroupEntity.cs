using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LevelGroup.
    /// </summary>
    [Serializable]
    public class LevelGroupEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelGroup"/> class.
        /// </summary>
        public LevelGroupEntity()
        {
            LevelLimits = new List<LevelLimitEntity>();
            LT_LevelGroups = new List<LtLevelGroupEntity>();
        }

        /// <summary>
        /// Gets or sets the level group identifier.
        /// </summary>
        /// <value>The level group identifier.</value>
        public int LevelGroupId { get; set; }
        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The activity identifier.</value>
        public int ActivityId { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        public int? CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the departmentid.
        /// </summary>
        /// <value>The departmentId.</value>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the roleid.
        /// </summary>
        /// <value>The roleid.</value>
        public int? RoleId { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        public virtual ActivityEntity Activity { get; set; }
        /// <summary>
        /// Gets or sets the level limits.
        /// </summary>
        /// <value>The level limits.</value>
        public virtual ICollection<LevelLimitEntity> LevelLimits { get; set; }
        /// <summary>
        /// Gets or sets the l t_ level group.
        /// </summary>
        /// <value>The l t_ level group.</value>
        public virtual ICollection<LtLevelGroupEntity> LT_LevelGroups { get; set; }
    }
}
